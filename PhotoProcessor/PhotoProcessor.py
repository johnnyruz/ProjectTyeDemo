
import os
import pika
from pymongo import MongoClient
from bson.objectid import ObjectId
from PIL import Image
import json
from io import BytesIO
import re, time, base64
from PIL.ImageFilter import (
   SHARPEN
)

def main():

    if 'RMQ_USER' in os.environ:
        rmq_user = os.environ['RMQ_USER']
    else:
        rmq_user = "user"

    if 'RMQ_PASS' in os.environ:
        rmq_pass = os.environ['RMQ_PASS']
    else:
        rmq_pass = "pass"

    if 'SERVICE__RABBITMQ__RMQ__HOST' in os.environ:
        rmq_host = os.environ['SERVICE__RABBITMQ__RMQ__HOST']
    else:
        rmq_host = "localhost"

    print(str(time.time()) + ": " + rmq_host)

    if 'CONNECTIONSTRINGS__MONGO' in os.environ:
        mongo_conn_string = os.environ['CONNECTIONSTRINGS__MONGO']
    else:
        mongo_conn_string = "mongodb://localhost:27017"

    if 'PROCESS_DELAY_TIME' in os.environ:
        process_delay_seconds = int(os.environ['PROCESS_DELAY_TIME'])
    else:
        process_delay_seconds = 5

    print(str(time.time()) + ": " + mongo_conn_string)

    credentials = pika.PlainCredentials(username=rmq_user, password=rmq_pass)
    connection = pika.BlockingConnection(pika.ConnectionParameters(host=rmq_host, credentials=credentials))
    channel = connection.channel()

    channel.exchange_declare("MessageContracts:PhotoAdded", "fanout", durable=True)
    channel.exchange_declare("photo-processor-listener", 'fanout', durable=True)
    channel.queue_declare("photo-processor-listener", durable=True)
       
    channel.exchange_bind("photo-processor-listener", "MessageContracts:PhotoAdded")
    channel.queue_bind("photo-processor-listener", "photo-processor-listener", routing_key = "")

    client = MongoClient(mongo_conn_string)
    db = client['photos']
    photos = db['photos']

    def publishProcessingComplete(photoId):
        channel.basic_publish(exchange="MessageContracts:ProcessingComplete",
                      routing_key='hello',
                      body='{ "message": { "photoId": "' + photoId + '"}, "messageType": ["urn:message:MessageContracts:ProcessingComplete"] }')


    def callback(ch, method, properties, body):
        time.sleep(process_delay_seconds)
        msgObj = json.loads(body)
        photoId = msgObj['message']['photoId']
        p = photos.find_one({"_id": ObjectId(photoId)})
        base64_data = re.sub('^data:image/.+;base64,', '', p['OriginalImageData'])
        byte_data = base64.b64decode(base64_data)
        image_data = BytesIO(byte_data)
        img = Image.open(image_data)
        processed_img = img.resize((800, 800)).filter(SHARPEN)
        buffered = BytesIO()
        processed_img.save(buffered, format="JPEG")
        buffered.seek(0)
        img_byte = buffered.getvalue()
        img_str = "data:image/jpeg;base64," + base64.b64encode(img_byte).decode()
        photos.update_one({"_id": ObjectId(photoId)}, { "$set": { 'ProcessedImageData': img_str, 'IsProcessed': True }})
        t = time.time()
        publishProcessingComplete(photoId)
        print(str(t) + " - Processed image: " + photoId)


    channel.basic_consume(queue="photo-processor-listener", on_message_callback=callback, auto_ack=True)

    print(' [*] Waiting for messages. To exit press CTRL+C')
    channel.start_consuming()

if __name__ == '__main__':
    try:
        main()
    except KeyboardInterrupt:
        print('Interrupted')
        try:
            sys.exit(0)
        except SystemExit:
            os._exit(0)



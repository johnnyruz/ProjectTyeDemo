FROM python:3.8-slim-buster
# Copy and install requirements for the app
COPY requirements.txt ./
RUN pip install --no-cache-dir -r requirements.txt

COPY . .

# Execute the program
CMD ["python", "PhotoProcessor.py"]
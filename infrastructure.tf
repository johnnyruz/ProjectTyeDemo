provider "azurerm" {
    features {}
}

variable "PROJECT_NAME" {
    type = string
}

# create resource group
resource "azurerm_resource_group" "rg" {
    name = var.PROJECT_NAME
    location = "eastus"
}

resource "azurerm_container_registry" "acr" {
    name = "${var.PROJECT_NAME}acr"
    location = azurerm_resource_group.rg.location
    resource_group_name = azurerm_resource_group.rg.name
    sku = "Basic"
    admin_enabled = true
}

resource "azurerm_kubernetes_cluster" "aks" {
  name = "${var.PROJECT_NAME}-aks"
  location = azurerm_resource_group.rg.location
  resource_group_name = azurerm_resource_group.rg.name
  dns_prefix = "${var.PROJECT_NAME}-aks-dns"
  kubernetes_version = "1.20.9"

  addon_profile {
    azure_policy {
      enabled = false
    }
    http_application_routing {
      enabled = true
    }              
  }

  default_node_pool {
    name = "default"
    vm_size = "Standard_D2as_v4"
    node_count = 2
  }

  identity {
    type = "SystemAssigned"
  }

  network_profile {
    network_plugin = "azure"
    dns_service_ip = "10.0.0.10"
    docker_bridge_cidr = "172.17.0.1/16"
    service_cidr = "10.0.0.0/16"
  }
}

resource "azurerm_role_assignment" "acrpull_role" {
  scope                            = azurerm_container_registry.acr.id
  role_definition_name             = "AcrPull"
  principal_id                     = azurerm_kubernetes_cluster.aks.kubelet_identity[0].object_id
  skip_service_principal_aad_check = true
}

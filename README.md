# KeyForge

## Project description 
<br> KeyForge is an API-first, microservices-based platform that provides secure, multi-tenant vaults for managing cryptographic keys and secrets. Built around a strict domain-driven architecture, KeyForge lets individuals and teams spin up isolated “vaults,” 
store and rotate keys with fine-grained policies, and collaborate safely via invitation workflows and real-time audit feeds. Every action—creating a vault, issuing a key, 
accessing a secret—is immutably logged and can be streamed live to dashboards for compliance and security monitoring.

# 🔧 API Setup Guide (Development Environment)

This project uses **User Secrets** to store local connection strings securely.  
Follow the steps below to configure each API and apply the required database migrations.

## 📁 1. Configure Connection Strings Using User Secrets

Each API has its own database connection string. These **must be added locally** using Visual Studio’s **User Secrets** feature.

### **Steps**
1. Right-click the desired API project in Visual Studio  
2. Select **Manage User Secrets**  
3. In the generated `secrets.json` file, paste the appropriate entry  
4. Replace the empty string `""` with your local SQL Server connection string

## 🔑 2. Required Connection String Entries

### **Account API**
```json  
"ConnectionStrings": {
  "AccountAPIConnection": ""
} 
```

### **Auth API**
```json
"ConnectionStrings": {
  "AuthAPIConnection": ""
}
 ```

### **Vault API**
```json
"ConnectionStrings": {
  "VaultApiConnectionString": ""
}
```

### **Vault Keys API**
```json
"ConnectionStrings": {
  "VaultKeysApiConnectionString": ""
}
```

# 🗄️ Applying Entity Framework Core Migrations (Per API via .NET CLI)

Each API in the solution maintains its own database.  
After adding your connection strings using User Secrets, run the following commands to apply migrations for each API using the **.NET CLI**.

## 🚀 Apply Migrations Using .NET CLI

Navigate to the solution root or the API project directory, then run the corresponding command for each API. If using visual studio, go to tools -> NuGet Packet Manager -> Package Manager Console

### Reminder to restore packages before migrations
Packages may need to be restored before applying migrations, simply go to the package manager console and do: 
```bash
dotnet restore
```

### **Account API**
```bash
dotnet ef database update --project AccountAPI
```
### **Auth API**
```bash
dotnet ef database update --project AuthAPI
```
### **Vault API**
```bash
dotnet ef database update --project VaultAPI
```
### **Vault Keys API**
```bash
dotnet ef database update --project VaultKeysAPI
```
# 🛠️ Current Working Build Projects

## **Steps to setup current build**
1. Navigate to "Start"
2. Click the dropdown arrow 
3. Common properties  
4. Configure startup projects
5. Select multiple startup projects
6. Under "Action" select "Start" on the AccountAPI
7. Under "Action" select "Start" on the AuthAPI
8. Under "Action" select "Start" on the VaultAPI
9. Under "Action" select "Start" on the VaultKeysAPI
10. Under "Action" select "Start" on the "ReactClient"
   



# KutCode.Security.Ldap

## ℹ️ Info
Web API microservice with only one purpose:  
Make LDAP auth integration simplier.

## 🧑‍⚖️ License
Please read the simple [MIT license](./LICENSE).

## 📜 Stack
- Dotnet 8
- FastEndpoints
- Swagger
- Docker

## 🎁 Packages
Use below packages for client integration with service API
- [KutCode.Security.Ldap](https://www.nuget.org/packages/KutCode.Security.Ldap)
- [KutCode.Security.Ldap.DependencyInjection](https://www.nuget.org/packages/KutCode.Security.Ldap.DependencyInjection)

## 📦 Installation
Use `docker compose` to setup container ease.  
Shure, you can produce manual installation with dotnet-runtime.

### Docker
#### Docker run
```bash
cd ./src/presentation/KutCode.Security.Ldap.WebApi
docker build -t ldap .
docker run -d -p 8080:8080 -v ./appsettings:/app/appsettings -v ./logs:/apt/logs -e ASPNETCORE_URLS=http://+:80 ldap
```
#### Docker compose
Edit `docker-compose.yml` file:
```bash 
services: 
    webapi:
        #image: registry.domain.local/ldap
        build:
            # set path to source code project directory
            context: src/presentation/KutCode.Security.Ldap.WebApi
        ports: 
            - 9080:8080
        environment:
            ASPNETCORE_ENVIRONMENT: Production
            ListenPort: 8080 ## not required, 8080 by default
        volumes:
          - ./appsettings:/app/appsettings
          - ./logs:/app/logs
```
Execute from the solution root directory:
```bash
docker compose up -d  
```

#### Verify installation
To check installation open in browser:  
`http://localhost:[your port]/swagger`  
OR  
```bash
curl http://[ip]:[port]/swagger -v
```



## ⚙️ Configuration
### Application settings configuration
In application root `/appsettings` directory create `appsettings.json` file with following content:
```json
{
  "Culture": "en",
  "ListenPort": 80,
  "Ldap": {
    "Server": "dc01.examplpe.local",
    "ServerPort": 389,
    "DomainName": "examplpe.local",
    "BaseLdapFilter": "DC=examplpe,DC=local",
    "AdditionalLdapFilter": "&(objectClass=user)(objectClass=person)",
    "LoginAttribute": "sAMAccountName",
    "DisplayNameAttribute": "displayName",
    "UseSsl": false,
    "ServiceAccount": {
      "Username": "domain-login",
      "Password": "password"
    }
  },
  "Rpc":{
    "Enabled": false,
    "Port": 9081,
    "Secure": false
  },
  "Cors": {
    "Origins": [
      "localhost", "some-one-else.com"
    ]
  }
}
```
Here some information about this settings:
- `Culture` - language of validation messages
- `ListenPort` - port to listen on, for webApi only
- `Ldap`
  - `Server` - LDAP server name or ip-address
  - `ServerPort` - LDAP server port, 389 is default non-ssl LDAP port 
  - `DomainName` - Domain name of LDAP instance
  - `BaseLdapFilter` - LDAP base filter for user search
  - `AdditionalLdapFilter` - LDAP additional filter for user search
  - `LoginAttribute` - LDAP login attribute
  - `DisplayNameAttribute` - LDAP display name attribute
  - `EmailAttribute` - LDAP email attribute
  - `UseSsl` - Should LDAP connection use ssl
  - `ServiceAccount` - Account to load domain users *(don't forget about account domain read permissions)*
    - `Username` - Domain account username
    - `Password` - Domain account password
- `Rpc` - gRPC settings
  - `Enabled` - will application accept gRPC connections
  - `Port` - port for HTTP2 connections (cause gRPC works on HTTP2), do not set same port that Web-API use if connections will be not secured with TLS
  - `Secure` - does application will accept secured connections (allows to use HTTP API and gRPC on the same port)
- `Cors`
  - `Origins` - list of allowed origins, use `localhost` by default,
  and add some custom origins if application has access to browser url  

## 🏃‍♂️ Usage

After launching the application, you can access the web api from the browser using `Swagger UI`:  
`http://localhost:[your port]/swagger`

Here is methods description:
- GET: `/api/v1/ping` - check if service is up
- POST: `/api/v1/auth` - authenticate user with LDAP by login/password
- GET: `/api/v1/objects/users` - get all domain users *(may take long time to load, based on domain size)*

### POST `/api/v1/auth` schemes
JSON request body:
```json
{
  "login": "example_user", // user Domain login
  "password": "example_password" // user Domain password
}
```
JSON response:
```json
{
  "status": "OK",
  "code": 200,
  "value": {
    "authorized": true, // is user authorized success
    "userData": {
      "userId": "1.3.3.2.2.1.4554.1.22.3", // LDAP unique identity
      "userDistinguishedName": "CN=Example User,OU=Some group,OU=Users,DC=somedomain,DC=local",
      "userDisplayName": "Example User",
      "memberOfGroups": [ // the name of the groups that the user is a member of
        "some_groups"
      ]
    }
  }
}
```
TypeScript model:
```ts
export interface LdapAuthResponse {
  status: string
  code: number
  value: AuthResult | null
}

export interface AuthResult {
  authorized: boolean
  userData: UserData
}

export interface UserData {
  userId: string
  userDistinguishedName: string
  userDisplayName: string
  memberOfGroups: string[]
}
```
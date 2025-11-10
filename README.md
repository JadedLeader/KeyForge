# KeyForge

## Project description 
<br> KeyForge is an API-first, microservices-based platform that provides secure, multi-tenant vaults for managing cryptographic keys and secrets. Built around a strict domain-driven architecture, KeyForge lets individuals and teams spin up isolated “vaults,” 
store and rotate keys with fine-grained policies, and collaborate safely via invitation workflows and real-time audit feeds. Every action—creating a vault, issuing a key, 
accessing a secret—is immutably logged and can be streamed live to dashboards for compliance and security monitoring.

## Database connection string setup
<br> Since this is a microservices project, you will have to ensure that the secrets in each project satisfy the connection string needs on your local machine, ensure that names match what's designated to each service within the program.cs of each project (yes, this is annoying as of current, this needs to be changed)

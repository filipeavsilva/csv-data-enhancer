# Deployment plan

This document describes how this solution should be deployed if for a production system.

## Building
Building and deploying the solution should be done by a CI/CD pipeline, such as Github Actions, Azure DevOps or e.g. TeamCity. Ideally this would happen in a  continuous deployment fashion, once a pull request was merged into the main branch (after proper code review and test running).

I would suggest github actions for simplicity of setup.

## API
The API itself is relatively simple and self-contained, having only a single executable part. Depending on users' requirements on availability and performance, and taking into account cost considerations, I would deploy this API to the cloud. Using Azure terms (which is the public cloud I'm more comfortable with), I would suggest either using an App service or a (serverless) Azure Function.

The App service is essentially always available and running, without "cold start" ramp-up times. However, if the usage of this API is very sporadic, it will often be idle, with the possibility of costing significantly more than it should given the usage.

An Azure function will be started up when a request comes in, and stopped after it's finished. This could be an advantage for infrequent usage, since we would only be billed for the actual time being used. However, especially if a request is the first in a while, it could take longer to get a response, which could be an issue. On the other hand, if a serverless function is used heavily, the costs can be higher than the App service.
Furthermore, the code would need to be changed to fit a serverless deployment flow. 

Without knowing much more about the expected usage patterns, but given my view of the workflow of the users (working with csv files) and the general performance of the API, I would suggest starting with a serverless function, and monitor it closely to assess if the usage pattern makes sense for it.

## Logging and monitoring

While not implemented in the current project, I would suggest publishing both logs and metrics to a monitoring tool such as Datadog, Grafana, Splunk, etc. All of these solutions have several tradeoffs, especially on hosting, cost, and out-of-the-box functionality. I would initially suggest Datadog, even though it's not cheap (but I assume it would not be used only for this API, but rather for the whole organization).

Logs can be published to these tools using structured logging to maximize the information included in logs, with a library like Serilog.
Metrics and traces can use the OpenTelemetry standard, which is now officially supported in .NET.

While logs would be useful to understand any issues that might occur, I would initially focus more on metrics (e.g. number of requests received, input data size, number of requests made to GLEIF, etc.) to ensure the understanding of the usage patterns of the API.

## Security

Again, something that was not my focus on this assignment.

If deploying this API to production, I would add access control based on an identity service such as Azure Active Directory. Ideally only authenticated Cardano users would be able to access the API. This could be achieved via an Oauth2.0 flow. Depending on the users' tolerance for inconvenience and preferred usage, a client application or a more advanced script could be developed to automate as much as possible the authentication procedure.

Another possible security measure (although not enough by itself to fully secure the API) would be to restrict access to the API to a specific Cardano-owned network or VPN, with something like Azure API Management.
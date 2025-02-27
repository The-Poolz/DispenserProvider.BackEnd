# DispenserProvider.BackEnd

## Overview

The **DispenserProvider.BackEnd** repository contains a Lambda function that acts as the handler for endpoints in the `API4.Dispenser` GraphQL API.
The Lambda function accepts one of several supported queries and, based on the request, invokes the appropriate handler.

## Key Features

- **MediatR Integration:**  
  Utilizes the `MediatR` library to route incoming requests to their corresponding handlers.

- **Automatic Request Validation:**  
  Incorporates `MediatR.Extensions.FluentValidation.AspNetCore` for automatic validation of requests.
  This means that the request validation is seamlessly handled in the pipeline without explicit calls to validators within the handler code.

## Running Locally
To test the Lambda function locally, you can use the AWS Lambda .NET Mock Lambda Test Tool.
For detailed instructions on local testing and debugging, refer to the [Lambda Documentation](https://github.com/The-Poolz/DispenserProvider.BackEnd/blob/master/src/DispenserProvider/Readme.md)
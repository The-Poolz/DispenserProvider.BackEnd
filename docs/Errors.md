# Error Documentation

This document describes the various errors that may occur in the system.
It provides details on the endpoint, handler file, error code, error message, and optional error information.

## Overview

### Internal errors

#### **Error Code**: INVALID_STAGE
- **Endpoint(s)**: Any endpoint.
- **Error Message**: No one valid stage in 'PRODUCTION_MODE' environment variable found.
- **JSON Example**:

<details>
  <summary>Show JSON Example</summary>

  ```json
  {
      "data": null,
      "errors": [
          {
              "path": [
                  "generateSignature"
              ],
              "data": null,
              "errorType": "INVALID_STAGE",
              "errorInfo": null,
              "locations": [ ],
              "message": "No one valid stage in 'PRODUCTION_MODE' environment variable found."
          }
      ]
  }
  ```

</details>

---

#### **Error Code**: INVALID_HANDLER_REQUEST
- **Endpoint(s)**: Any endpoint.
- **Error Message**: No one implemented request found.
- **JSON Example**:

<details>
  <summary>Show JSON Example</summary>

  ```json
  {
      "data": null,
      "errors": [
          {
              "path": [
                  "generateSignature"
              ],
              "data": null,
              "errorType": "INVALID_HANDLER_REQUEST",
              "errorInfo": null,
              "locations": [ ],
              "message": "No one implemented request found."
          }
      ]
  }
  ```

</details>

---

#### **Error Code**: RECOVERED_ADDRESS_IS_INVALID
- **Endpoint(s)**: `adminCreateAsset`, `adminDeleteAsset`
- **Error Message**: Recovered address is not valid.
- **Optional ErrorInfo**: 
```json
{
    "RecoveredAddress": "0x.."
}
```
- **JSON Example**:

<details>
  <summary>Show JSON Example</summary>

  ```json
  {
      "data": null,
      "errors": [
          {
              "path": [
                  "adminCreateAsset"
              ],
              "data": null,
              "errorType": "RECOVERED_ADDRESS_IS_INVALID",
              "errorInfo": {
                  "RecoveredAddress": "0x.."
              },
              "locations": [ ],
              "message": "Recovered address is not valid."
          }
      ]
  }
  ```

</details>

---

#### **Error Code**: USERS_COLLECTION_IS_EMPTY
- **Endpoint(s)**: `adminCreateAsset`, `adminDeleteAsset`
- **Error Message**: Collection of users must contain 1 or more elements.
- **JSON Example**:

<details>
  <summary>Show JSON Example</summary>

  ```json
  {
      "data": null,
      "errors": [
          {
              "path": [
                  "adminCreateAsset"
              ],
              "data": null,
              "errorType": "USERS_COLLECTION_IS_EMPTY",
              "errorInfo": null,
              "locations": [ ],
              "message": "Collection of users must contain 1 or more elements."
          }
      ]
  }
  ```

</details>

---

#### **Error Code**: USERS_COLLECTION_CONTAIN_DUPLICATES
- **Endpoint(s)**: `adminCreateAsset`, `adminDeleteAsset`
- **Error Message**: Collection of users contain duplicates.
- **JSON Example**:

<details>
  <summary>Show JSON Example</summary>

  ```json
  {
      "data": null,
      "errors": [
          {
              "path": [
                  "adminCreateAsset"
              ],
              "data": null,
              "errorType": "USERS_COLLECTION_CONTAIN_DUPLICATES",
              "errorInfo": null,
              "locations": [ ],
              "message": "Collection of users contain duplicates."
          }
      ]
  }
  ```

</details>

---

#### **Error Code**: USERS_COLLECTION_MUST_BE_SORTED
- **Endpoint(s)**: `adminCreateAsset`, `adminDeleteAsset`
- **Error Message**: Collection of users must be sorted by ascending.
- **JSON Example**:

<details>
  <summary>Show JSON Example</summary>

  ```json
  {
      "data": null,
      "errors": [
          {
              "path": [
                  "adminCreateAsset"
              ],
              "data": null,
              "errorType": "USERS_COLLECTION_MUST_BE_SORTED",
              "errorInfo": null,
              "locations": [ ],
              "message": "Collection of users must be sorted by ascending."
          }
      ]
  }
  ```

</details>

---

#### **Error Code**: SCHEDULE_IS_EMPTY
- **Endpoint(s)**: `adminCreateAsset`
- **Error Message**: Schedule must contain 1 or more elements.
- **JSON Example**:

<details>
  <summary>Show JSON Example</summary>

  ```json
  {
      "data": null,
      "errors": [
          {
              "path": [
                  "adminCreateAsset"
              ],
              "data": null,
              "errorType": "SCHEDULE_IS_EMPTY",
              "errorInfo": null,
              "locations": [ ],
              "message": "Schedule must contain 1 or more elements."
          }
      ]
  }
  ```

</details>

---

#### **Error Code**: SCHEDULE_MUST_BE_SORTED
- **Endpoint(s)**: `adminCreateAsset`
- **Error Message**: Schedule must be sorted in ascending order by 'StartDate'.
- **JSON Example**:

<details>
  <summary>Show JSON Example</summary>

  ```json
  {
      "data": null,
      "errors": [
          {
              "path": [
                  "adminCreateAsset"
              ],
              "data": null,
              "errorType": "SCHEDULE_MUST_BE_SORTED",
              "errorInfo": null,
              "locations": [ ],
              "message": "Schedule must be sorted in ascending order by 'StartDate'."
          }
      ]
  }
  ```

</details>

---

#### **Error Code**: SUM_OF_RATIOS_MUST_BE_ONE
- **Endpoint(s)**: `adminCreateAsset`
- **Error Message**: The sum of the ratios must be 1.
- **JSON Example**:

<details>
  <summary>Show JSON Example</summary>

  ```json
  {
      "data": null,
      "errors": [
          {
              "path": [
                  "adminCreateAsset"
              ],
              "data": null,
              "errorType": "SUM_OF_RATIOS_MUST_BE_ONE",
              "errorInfo": null,
              "locations": [ ],
              "message": "The sum of the ratios must be 1."
          }
      ]
  }
  ```

</details>

---

#### **Error Code**: FIRST_ELEMENT_MUST_BE_TGE
- **Endpoint(s)**: `adminCreateAsset`
- **Error Message**: The first element must be the TGE (Token Generation Event).
- **JSON Example**:

<details>
  <summary>Show JSON Example</summary>

  ```json
  {
      "data": null,
      "errors": [
          {
              "path": [
                  "adminCreateAsset"
              ],
              "data": null,
              "errorType": "FIRST_ELEMENT_MUST_BE_TGE",
              "errorInfo": null,
              "locations": [ ],
              "message": "The first element must be the TGE (Token Generation Event)."
          }
      ]
  }
  ```

</details>

---

#### **Error Code**: MINIMUM_RATIO_1E_MINUS_18
- **Endpoint(s)**: `adminCreateAsset`
- **Error Message**: Ratio must be greater than or equal to 1e-18.
- **JSON Example**:

<details>
  <summary>Show JSON Example</summary>

  ```json
  {
      "data": null,
      "errors": [
          {
              "path": [
                  "adminCreateAsset"
              ],
              "data": null,
              "errorType": "MINIMUM_RATIO_1E_MINUS_18",
              "errorInfo": null,
              "locations": [ ],
              "message": "Ratio must be greater than or equal to 1e-18."
          }
      ]
  }
  ```

</details>

---

#### **Error Code**: END_TIME_MUST_BE_GREATER_THAN_START_TIME
- **Endpoint(s)**: `adminCreateAsset`
- **Error Message**: End time must be greater than start time.
- **JSON Example**:

<details>
  <summary>Show JSON Example</summary>

  ```json
  {
      "data": null,
      "errors": [
          {
              "path": [
                  "adminCreateAsset"
              ],
              "data": null,
              "errorType": "END_TIME_MUST_BE_GREATER_THAN_START_TIME",
              "errorInfo": null,
              "locations": [ ],
              "message": "End time must be greater than start time."
          }
      ]
  }
  ```

</details>

---

#### **Error Code**: INVALID_TOKEN_OWNER
- **Endpoint(s)**: `adminCreateAsset`
- **Error Message**: Owner of provided PoolId in the provided ChainId is invalid.
- **Optional ErrorInfo**: 
```json
{
    "ChainId": 56,
    "PoolId": 123
}
```
- **JSON Example**:

<details>
  <summary>Show JSON Example</summary>

  ```json
  {
      "data": null,
      "errors": [
          {
              "path": [
                  "adminCreateAsset"
              ],
              "data": null,
              "errorType": "INVALID_TOKEN_OWNER",
              "errorInfo": {
                  "ChainId": 56,
                  "PoolId": 123
              },
              "locations": [ ],
              "message": "Owner of provided PoolId in the provided ChainId is invalid."
          }
      ]
  }
  ```

</details>

---

#### **Error Code**: BUILDER_MUST_BE_APPROVED_IN_LOCK_DEAL_NFT
- **Endpoint(s)**: `adminCreateAsset`
- **Error Message**: Provided builder address not approved in the LockDealNFT contract.
- **Optional ErrorInfo**: 
```json
{
    "ChainId": 56,
    "Address": "0x..."
}
```
- **JSON Example**:

<details>
  <summary>Show JSON Example</summary>

  ```json
  {
      "data": null,
      "errors": [
          {
              "path": [
                  "adminCreateAsset"
              ],
              "data": null,
              "errorType": "BUILDER_MUST_BE_APPROVED_IN_LOCK_DEAL_NFT",
              "errorInfo": {
                  "ChainId": 56,
                  "Address": "0x..."
              },
              "locations": [ ],
              "message": "Provided builder address not approved in the LockDealNFT contract."
          }
      ]
  }
  ```

</details>

---

#### **Error Code**: BUILDER_MUST_BE_SIMPLE_PROVIDER
- **Endpoint(s)**: `adminCreateAsset`
- **Error Message**: Provided builder address is not a simple provider.
- **Optional ErrorInfo**: 
```json
{
    "ChainId": 56,
    "Address": "0x..."
}
```
- **JSON Example**:

<details>
  <summary>Show JSON Example</summary>

  ```json
  {
      "data": null,
      "errors": [
          {
              "path": [
                  "adminCreateAsset"
              ],
              "data": null,
              "errorType": "BUILDER_MUST_BE_SIMPLE_PROVIDER",
              "errorInfo": {
                  "ChainId": 56,
                  "Address": "0x..."
              },
              "locations": [ ],
              "message": "Provided builder address is not a simple provider."
          }
      ]
  }
  ```

</details>

---

#### **Error Code**: SIGNATURE_IS_STILL_VALID
- **Endpoint(s)**: `generateSignature`
- **Error Message**: Cannot generate signature, because it is still valid until.
- **Optional ErrorInfo**: 
```json
{
    "ValidFrom": 123456789,
    "NextTry": 123456789,
    "CurerntTime": 123456789
}
```
- **JSON Example**:

<details>
  <summary>Show JSON Example</summary>

  ```json
  {
      "data": null,
      "errors": [
          {
              "path": [
                  "generateSignature"
              ],
              "data": null,
              "errorType": "SIGNATURE_IS_STILL_VALID",
              "errorInfo": {
                  "ValidFrom": 123456789,
                  "NextTry": 123456789,
                  "CurerntTime": 123456789
              },
              "locations": [ ],
              "message": "Cannot generate signature, because it is still valid until."
          }
      ]
  }
  ```

</details>

---

#### **Error Code**: SIGNATURE_GENERATION_VALID_TIME_NOT_ARRIVED
- **Endpoint(s)**: `generateSignature`
- **Error Message**: Cannot generate signature, because the next valid time for generation has not yet arrived.
- **Optional ErrorInfo**: 
```json
{
    "NextTry": 123456789,
    "CurerntTime": 123456789
}
```
- **JSON Example**:

<details>
  <summary>Show JSON Example</summary>

  ```json
  {
      "data": null,
      "errors": [
          {
              "path": [
                  "generateSignature"
              ],
              "data": null,
              "errorType": "SIGNATURE_GENERATION_VALID_TIME_NOT_ARRIVED",
              "errorInfo": {
                  "NextTry": 123456789,
                  "CurerntTime": 123456789
              },
              "locations": [ ],
              "message": "Cannot generate signature, because the next valid time for generation has not yet arrived."
          }
      ]
  }
  ```

</details>

---

#### **Error Code**: REFUND_TIME_IS_EXPIRED
- **Endpoint(s)**: `generateSignature`
- **Error Message**: Cannot generate signature for refund, because refund time has expired.
- **Optional ErrorInfo**: 
```json
{
    "RefundFinishTime": 123456789,
    "CurerntTime": 123456789
}
```
- **JSON Example**:

<details>
  <summary>Show JSON Example</summary>

  ```json
  {
      "data": null,
      "errors": [
          {
              "path": [
                  "generateSignature"
              ],
              "data": null,
              "errorType": "REFUND_TIME_IS_EXPIRED",
              "errorInfo": {
                  "RefundFinishTime": 123456789,
                  "CurerntTime": 123456789
              },
              "locations": [ ],
              "message": "Cannot generate signature for refund, because refund time has expired."
          }
      ]
  }
  ```

</details>

---

#### **Error Code**: ASSET_ALREADY_WITHDRAWN
- **Endpoint(s)**: `generateSignature`, `retriveSignature`
- **Error Message**: Cannot generate signature, because asset already withdrawn.
- **JSON Example**:

<details>
  <summary>Show JSON Example</summary>

  ```json
  {
      "data": null,
      "errors": [
          {
              "path": [
                  "generateSignature"
              ],
              "data": null,
              "errorType": "ASSET_ALREADY_WITHDRAWN",
              "errorInfo": null,
              "locations": [ ],
              "message": "Cannot generate signature, because asset already withdrawn."
          }
      ]
  }
  ```

</details>

---

#### **Error Code**: ASSET_ALREADY_REFUNDED
- **Endpoint(s)**: `generateSignature`, `retriveSignature`
- **Error Message**: Cannot generate signature, because asset already refunded.
- **JSON Example**:

<details>
  <summary>Show JSON Example</summary>

  ```json
  {
      "data": null,
      "errors": [
          {
              "path": [
                  "generateSignature"
              ],
              "data": null,
              "errorType": "ASSET_ALREADY_REFUNDED",
              "errorInfo": null,
              "locations": [ ],
              "message": "Cannot generate signature, because asset already refunded."
          }
      ]
  }
  ```

</details>

---

#### **Error Code**: DISPENSER_NOT_FOUND
- **Endpoint(s)**: `generateSignature`, `retriveSignature`
- **Error Message**: Asset by provided PoolId and ChainId for user, not found.
- **JSON Example**:

<details>
  <summary>Show JSON Example</summary>

  ```json
  {
      "data": null,
      "errors": [
          {
              "path": [
                  "generateSignature"
              ],
              "data": null,
              "errorType": "DISPENSER_NOT_FOUND",
              "errorInfo": null,
              "locations": [ ],
              "message": "Asset by provided PoolId and ChainId for user, not found."
          }
      ]
  }
  ```

</details>

---

#### **Error Code**: SIGNATURE_NOT_FOUND
- **Endpoint(s)**: `retriveSignature`
- **Error Message**: Signature for user, not found.
- **JSON Example**:

<details>
  <summary>Show JSON Example</summary>

  ```json
  {
      "data": null,
      "errors": [
          {
              "path": [
                  "retrieveSignature"
              ],
              "data": null,
              "errorType": "SIGNATURE_NOT_FOUND",
              "errorInfo": null,
              "locations": [ ],
              "message": "Signature for user, not found."
          }
      ]
  }
  ```

</details>

---

#### **Error Code**: SIGNATURE_VALID_TIME_NOT_ARRIVED
- **Endpoint(s)**: `retriveSignature`
- **Error Message**: Cannot retrieve signature, because the valid time for retrieving has not yet arrived.
- **Optional ErrorInfo**: 
```json
{
    "ValidFrom": 123456789,
    "CurerntTime": 123456789
}
```
- **JSON Example**:

<details>
  <summary>Show JSON Example</summary>

  ```json
  {
      "data": null,
      "errors": [
          {
              "path": [
                  "retrieveSignature"
              ],
              "data": null,
              "errorType": "SIGNATURE_VALID_TIME_NOT_ARRIVED",
              "errorInfo": {
                  "ValidFrom": 123456789,
                  "CurerntTime": 123456789
              },
              "locations": [ ],
              "message": "Cannot retrieve signature, because the valid time for retrieving has not yet arrived."
          }
      ]
  }
  ```

</details>

---

#### **Error Code**: SIGNATURE_VALID_TIME_IS_EXPIRED
- **Endpoint(s)**: `retriveSignature`
- **Error Message**: Cannot retrieve signature, because the valid time for using signature is expired.
- **Optional ErrorInfo**: 
```json
{
    "ValidUntil": 123456789,
    "CurerntTime": 123456789
}
```
- **JSON Example**:

<details>
  <summary>Show JSON Example</summary>

  ```json
  {
      "data": null,
      "errors": [
          {
              "path": [
                  "retrieveSignature"
              ],
              "data": null,
              "errorType": "SIGNATURE_VALID_TIME_IS_EXPIRED",
              "errorInfo": {
                  "ValidUntil": 123456789,
                  "CurerntTime": 123456789
              },
              "locations": [ ],
              "message": "Cannot retrieve signature, because the valid time for using signature is expired."
          }
      ]
  }
  ```

</details>

---

#### **Error Code**: SIGNATURE_TYPE_IS_INVALID
- **Endpoint(s)**: `retriveSignature`
- **Error Message**: Cannot retrieve signature, because it was generated for a other operation.
- **Optional ErrorInfo**: 
```json
{
    "IsRefund": 123456789,
    "CurerntTime": 123456789
}
```
- **JSON Example**:

<details>
  <summary>Show JSON Example</summary>

  ```json
  {
      "data": null,
      "errors": [
          {
              "path": [
                  "retrieveSignature"
              ],
              "data": null,
              "errorType": "SIGNATURE_TYPE_IS_INVALID",
              "errorInfo": {
                  "IsRefund": 123456789,
                  "CurerntTime": 123456789
              },
              "locations": [ ],
              "message": "Cannot retrieve signature, because it was generated for a other operation."
          }
      ]
  }
  ```

</details>

---
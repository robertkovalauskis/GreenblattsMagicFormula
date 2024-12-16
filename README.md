# Test Automation Paradigm

## Overview
UI, API, and Functional tests are structured around the application's functional modules. A **functional module** represents a logically complete feature used by the end user. A **business scenario** (or business flow) defines the sequence of user actions that interact with a functional module. These test types require periodic reviews and updates to align with evolving business requirements.
In contrast, **Unit Tests** are organized around the application's code architecture. They aim to cover as much backend logic as possible and typically require updates only when technical requirements change due to modifications in business needs.

---

## Test Types and Their Differences

The distinction between various test types lies in how they trigger the backend logic:

1. **UI Tests**: Interact with the application's front end (FE).
2. **API Tests**: Make direct calls to the relevant API endpoints.
3. **Functional Tests**: Directly call relevant methods (requires access to the application's code).
4. **Unit Tests**: Test the smallest logical pieces of code, such as individual methods.

---

## Test Pyramid

The sequence of these test types aligns with the **test pyramid**:

- **UI and API Tests** are at the top and should be the least numerous and represent the **external Black box** testing layer.
- **Functional Tests** and sit in the middle.
- **Unit Tests** form the base and should be the most numerous, representing together with **Functional tests** the **internal White box** testing layer.


### Key Principle:
An efficient test framework ensures that **Unit Tests** significantly outnumber **Functional** or **UI/API Tests**, maintaining a **balanced and scalable testing strategy**.

---

## Benefits of Following the Test Pyramid
- Faster execution with more **Unit Tests**.
- Reduced maintenance overhead with fewer **UI Tests**.
- Scalability and reliability through a proper distribution of test types.

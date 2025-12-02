# SimpleAuth

SimpleAuth started as a basic authentication API, but it now includes several components commonly found in modern systems used by larger platforms. The goal is not to build an overly complex or enterprise-heavy architecture, but rather something straightforward.

## User registration and login
Routes for registration and authentication using email and password were created.  
Passwords are protected using **PBKDF2**

The authentication flow issues two types of tokens:

- **Access token (JWT)** — short-lived, used to authorize authenticated requests.
- **Refresh token** — opaque, secure, generated with proper entropy using Base64URL.

## Refresh tokens and rotation
Each refresh token represents an active user session.  
The system implements **refresh token rotation**, meaning:

- the old refresh token is immediately revoked
- a new refresh token is generated
- a new access token is returned

## Login attempt tracking
A dedicated table was added to record every login attempt, including:

- email
- IP address
- timestamp
- success or failure

## Basic rate-limit and blocking
Using the stored attempts, the system applies a simple blocking mechanism:

- limits based on the number of failed attempts per email
- limits based on failed attempts per IP address
- uniform behavior even when the email does not exist

This prevents brute force attacks and reduces abuse without exposing whether an email is registered.

## Current state of the project
At this stage, SimpleAuth provides:

- user creation
- secure login with JWT
- refresh token rotation with automatic revocation
- detailed logging of authentication attempts
- basic rate-limits and safety checks
- a clean, extensible foundation for further improvements
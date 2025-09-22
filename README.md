# AAU Parking Service

This service automatically registers parking in Zone 4688 (Cassiopeia, Selma Lagerlöfs Vej 300, 9220 Aalborg Ø) at scheduled times.

## Features

- Automated parking registration via scheduled jobs
- Configurable via environment variables
- Retry logic for API requests using Polly
- Circuit Breaker pattern for handling transient faults
- Logging for job execution and API responses

**Disclaimer:**
    Please confirm all parking registrations via SMS from Apcoa. This app is offered without warranty - we cannot guarantee successful registrations and are not liable for any parking tickets or technical issues that may occur.

## Getting Started

### Prerequisites

- [Docker](https://docs.docker.com/get-started/get-docker/)
- [Docker Compose](https://docs.docker.com/compose/install/)
- [.NET 9 SDK](https://dotnet.microsoft.com/download) (for local development)

### Configuration

Set the following environment variables in the `compose.yml` file:

| Variable                      | Description                     | Example Value              |
|-------------------------------|---------------------------------|----------------------------|
| `HANGFIRE_DASHBOARD_USERNAME` | Username for Hangfire dashboard | `admin`                    |
| `HANGFIRE_DASHBOARD_PASSWORD` | Password for Hangfire dashboard | `password`                 |
| `PARKING_JOB_SCHEDULE`        | Cron expression for job schedule| `30 8 * * 1-5`             |
| `PARKING_JOB_PHONE_NUMBER`    | Danish phone number for registration without country code | `12345678`                 |
| `PARKING_JOB_LICENSE_PLATE`   | Vehicle license plate           | `AB12345`                  |

**Cron Expression Guide:**

- `* * * * *` - Every minute
- `*/5 * * * *` - Every 5 minutes
- `0 * * * *` - Every hour on the hour
- `0 8 * * 1-5` - At 08:00 AM, Monday to Friday

See [crontab.guru](https://crontab.guru/) for help creating cron expressions.

## Running the service with Docker Compose

1. **Building and starting the container:**

   ```bash
   docker compose up --build -d
   ```

2. **Start the container without rebuilding:**

   ```bash
   docker compose up -d
   ```

3. **Stop the container:**

   ```bash
   docker compose down
   ```

## Viewing Logs

1. **View recent logs:**

   ```bash
   docker compose logs aau-parking-scheduler
   ```

2. **Follow logs in real-time:**

   ```bash
   docker compose logs -f aau-parking-scheduler
   ```

3. **Viewing logs through Hangfire Dashboard:**

   Access the Hangfire dashboard at `http://localhost:8080/dashboard`. Log in using the credentials set in the environment variables. The dashboard provides insights into job execution history, success/failure rates, and allows manual triggering of jobs.

## Manual Triggering

1. **Hangfire Dashboard:**
    Access the Hangfire dashboard at `http://localhost:8080/dashboard`. Log in using the credentials set in the environment variables. Parking registrations can be manually triggered through the Hangfire dashboard recurring jobs.

![logo](doc/img/logo-banner.png)

# Reportify

[![CI](https://github.com/francWhite/reportify/actions/workflows/ci.yml/badge.svg?branch=main)](https://github.com/francWhite/reportify/actions/workflows/ci.yml)
[![GitHub release](https://img.shields.io/github/v/release/francWhite/reportify)](https://github.com/francWhite/reportify/releases)
[![licence](https://img.shields.io/github/license/francWhite/reportify)](https://github.com/francWhite/reportify/blob/main/LICENSE)

Reportify is a tool for generating reports based on tracked activities in [ManicTime](https://www.manictime.com/). It is designed to be used in conjunction with [Jira](https://www.atlassian.com/software/jira) to get the corresponding issue information, but can also be used without Jira.

## Table of Contents

- [Installation](#installation)
- [Usage](#usage)
- [License](#license)

## Installation

### Install using [winget](https://learn.microsoft.com/en-us/windows/package-manager/winget/) 

`winget install francWhite.reportify`
### Install manually

Download the latest release from the [releases page](https://github.com/francWhite/reportify/releases) and extract the archive to a folder of your choice.

## Usage

### Configuration

Before you can use Reportify, you need to configure it. This is done by editing the `.reportify` file your user directory which is created on the first start of Reportify. The file is in [YAML](https://yaml.org/) format and contains the following settings:

| Setting                  | Description                          | Example                                |
|--------------------------|--------------------------------------|----------------------------------------|
| `manicTime.databasePath` | Path to the ManicTime database file. | C:\Tools\ManicTime\ManicTimeReports.db |
| `jira.url`               | URL of the Jira instance.            | https://jira.atlassian.com             |
| `jira.accsessToken`      | Access token for the Jira instance.  | A1B2C3D4E5F6G                          |


Reportify will validate the configuration on startup and informs over missing or invalid settings.

#### Jira Access Token
The access token can be generated in Jira under `Profile > Personal Access Tokens > Create token`. Only the BROWSE_PROJECTS permission is required.

### Commands

Reportify supports generating reports for a specific date or the current week. If no parameters are specified, the report is generated for the current day.

```
USAGE:
    reportify.dll [OPTIONS]

OPTIONS:
    -h, --help           Prints help information
    -d, --date <DATE>    Create report for a specified date
    -w, --week           Create report for the current week
    -c, --copy           Copy report in CSV format to clipboard
```


**Example**

```
# .\reportify.exe -d 12.06.2023

── Report for 12.06.2023. Total: 08:51h ────────────────────────────────────────────────────
┌──────────┬──────────────┬────────────────────────────────────────────────────────────────┐
│ Position │ Duration (h) │ Activities                                                     │
├──────────┼──────────────┼────────────────────────────────────────────────────────────────┤
│ 123 456  │ 5,50 (05:23) │ - #REP-297 Validate configuration                     05:07    │
│          │              │ - #REP-263 Create new configuratio on start           00:16    │
│          │              │                                                                │
│ 234 567  │ 1,50 (01:31) │ - Administration $234567                              01:00    │
│          │              │ - Break $234567                                       00:31    │
│          │              │                                                                │
│ 345 678  │ 1,25 (01:09) │ #REP-291 Beautify report output                       01:09    │
│          │              │                                                                │
│ 456 789  │ 0,75 (00:46) │ #REP-228 Log Exceptions                               00:46    │
└──────────┴──────────────┴────────────────────────────────────────────────────────────────┘
```

### ManicTime

Reportify uses your local ManicTime database to get the tracked activities. If a Jira issue key is found in the activity description, the corresponding issue information is retrieved from Jira. The issue key (e.g. `#REP-228`) is extracted using the following regular expression: 
```regexp
#([a-zA-Z]+-\\d+)
```

If no Jira issue exists but you still want to track the time for a specific position, you can include the position number in the activity description (e.g. `$123456`). The position number is extracted using the following regular expression:
```regexp
\$(\d{6})
```


## License

Distributed under the GPL-3.0 License. View [`LICENSE`](https://github.com/francWhite/reportify/blob/main/LICENSE) for more information.

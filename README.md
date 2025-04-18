![logo](doc/img/logo-banner.png)

# reportify

[![CI](https://github.com/francWhite/reportify/actions/workflows/ci.yml/badge.svg?branch=main)](https://github.com/francWhite/reportify/actions/workflows/ci.yml)
[![GitHub release](https://img.shields.io/github/v/release/francWhite/reportify)](https://github.com/francWhite/reportify/releases)
[![licence](https://img.shields.io/github/license/francWhite/reportify)](https://github.com/francWhite/reportify/blob/main/LICENSE)

reportify is a tool for generating reports based on tracked activities in [ManicTime](https://www.manictime.com/). It is designed to be used in conjunction with [Jira](https://www.atlassian.com/software/jira) to get the corresponding issue information, but can also be used without it.

## Table of Contents

- [Installation](#installation)
- [Usage](#usage)
- [License](#license)

## Installation

### Install using [winget](https://learn.microsoft.com/en-us/windows/package-manager/winget/) 

`winget install francWhite.reportify`

### Install using [scoop](https://scoop.sh)

`scoop bucket add maple 'https://github.com/kevinboss/maple.git'`

`scoop install reportify`

### Install manually

Download the latest release from the [releases page](https://github.com/francWhite/reportify/releases) and extract the archive to a folder of your choice.

## Usage

### Configuration

Before you can use reportify, you need define some mandatory settings. This is done by editing the `.reportify` file in your user directory which is created on the first start of the application. The [YAML](https://yaml.org/) configuration file contains the following settings:

| Setting                  | Description                          | Example                                |
|--------------------------|--------------------------------------|----------------------------------------|
| `manicTime.databasePath` | Path to the ManicTime database file. | C:\Tools\ManicTime\ManicTimeReports.db |
| `jira.url`               | URL of the Jira instance.            | https://jira.atlassian.com             |
| `jira.accsessToken`      | Access token for the Jira instance.  | A1B2C3D4E5F6G                          |


reportify will validate the configuration on startup and informs over missing or invalid settings.

#### Jira Access Token
The access token can be generated in Jira under `Profile > Personal Access Tokens > Create token`. Only the BROWSE_PROJECTS permission is required.

### Commands

reportify supports generating reports for a specific date or an entire week. If no parameters are specified, the report is generated for the current day.

```
USAGE:
    reportify.dll [OPTIONS]

OPTIONS:
                                  DEFAULT
    -h, --help                               Prints help information
    -v, --version                            Prints version information
    -d, --date <DATE>                        Create report for a specified date, defaults to the current day
    -w, --week                               Create report for the current week
    -o, --week-offset <OFFSET>    0          Offset in weeks to determine the week used for the creation of the report
    -c, --copy                               Copy report in CSV format to clipboard
```


**Example**

```
# reportify -d 12.06.2023

── Report for 12.06.2023. Total: 9.00h (08:57) ────────────────────────────────────────────────────
┌──────────┬──────────────┬────────────────────────────────────────────────────────────────┐
│ Position │ Duration (h) │ Activities                                                     │
├──────────┼──────────────┼────────────────────────────────────────────────────────────────┤
│ 123 456  │ 5,50 (05:23) │ - #REP-297 Validate configuration                     05:07    │
│          │              │ - #REP-263 Create new configuratio on start           00:16    │
│          │              │                                                                │
│ 234 567  │ 1,50 (01:31) │ - Administration $234567                              01:00    │
│          │              │ - Break $234567                                       00:31    │
│          │              │                                                                │
│ 345 678  │ 1,25 (01:16) │ #REP-291 Beautify report output                       01:16    │
│          │              │                                                                │
│ 456 789  │ 0,75 (00:46) │ #REP-228 Log Exceptions                               00:46    │
└──────────┴──────────────┴────────────────────────────────────────────────────────────────┘
```

### ManicTime

Reportify uses your local ManicTime database to get the tracked activities. If a Jira issue key is found in the activity description, the corresponding issue information is retrieved from Jira. The issue key (e.g. `#REP-228`) is extracted using the following regular expression: 
```regexp
#([a-zA-Z]+-\\d+)
```

If no Jira issue exists, but you still want to track the time for a specific position, you can include the position number in the activity description (e.g. `$123456`). The position number is extracted using the following regular expression:
```regexp
\$(\d{6})
```

If you want to add a comment to the tracked activity you can do so by adding it after writing %, for example `#REP-228 %This is my comment`. The comment is extracted using the follwing regular expression:
```regexp
%(?<notes>.*)
```

Everything after the % will be treated as a comment so `%This is my comment #REP-228` will treat `This is my comment #REP-228` as the comment. Extracting the Jira issue will still work.


## License

Distributed under the GPL-3.0 License. View [`LICENSE`](https://github.com/francWhite/reportify/blob/main/LICENSE) for more information.

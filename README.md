# Terminal Pass
**Terminal Pass** is an open source, terminal based password manager written in C#.
It uses the latest **Aes 256** encryption standard to encrypt your data and saves it to a local file.

## Why Terminal Pass
 * AES 256
 * Open Source
 * Fully offline
 * Everything locally stored -> prevents any mitm attacks.
 * Completely free to use

# Getting started
Firstly, you need to [download](https://github.com/foreggs/terminal-pass/releases) one of the official releases or clone this repo and build it yourself.

Once you run the exe, it doesn't have to be installed and it can be stored anywhere you'd like. The first time you run it, it will ask you to create a password and then allow you to use a variety of simple commands to edit your password table.

![screenshot](https://i.imgur.com/pr36kmC.png)

# Basic Commands (more under `help` command)
 - **`help`** - List all the commands
 - **`add name password`** - adds an account row with a label name and a password.
 - **`delete name`** - deletes an account row by name.

## What if I want to back up my passwords to a cloud?
Pick any; Dropbox, Google Drive, OneDrive, any cloud storage service that allows you to save text files. All you need to do is navigate to *C:\Users\user\terminal-pass.txt* and copy this file to your cloud. If you'd like to import your passwords from the cloud you will need to drop your passwords file in the same directory under the same name.

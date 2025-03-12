### Project Dollhouse is a reconstruction of The Sims Online from the ground up in C#

**How to run**
First of all, you need to install TSO from [The Internet Archive](https://archive.org/search?query=the+sims+online)
To run the server, you need to  [install Docker for your platform.](https://docs.docker.com/desktop/setup/install/windows-install/)
Then cd into directory in the terminal, and type docker run -d -p 3077:3077 -p 8080:80 dollhouseserver

This will spawn a server that listens on port 3077.

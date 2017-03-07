1. update apt-get

```
cd /etc/apt/sources.list.d/
wget http://archive.cloudera.com/impala/ubuntu/precise/amd64/impala/cloudera.list
sudo apt-get update
```

2. Install Impala [found here [1]]

```
sudo apt-get install impala             # Binaries for daemons
sudo apt-get install impala-server      # Service start/stop script
sudo apt-get install impala-state-store # Service start/stop script
sudo apt-get install impala-shell

sudo service impala-server start
sudo service impala-state-store start

If you ever need to reboot your server, or create a new server from an image, you'll want to make sure that all of your critical processes come back online with your freshly booted server.  With CentOS/RHEL this is fairly easy, you would just use chckconfig, for Ubuntu you can use a process called update-rc.d.
First you'll want to see what services are avialable for startup.  You can do this by running:
 
 
service --status-all
The services that have a [+] - (service name) will start on boot.  The ones with [-] - (service name) will not start on boot. Adding a service to Ubuntu or Debian is done with the update-rc.d command. You can specify which runlevels to start and stop the new service or accept the defaults. The init.d file will be added to the relevant rc.d startup folders.

update-rc.d SERVICENAME defaults

To remove a service from start on boot:
  
update-rc.d -f  SERVICENAME remove

```
3. Install java and export JAVA_HOME

```
export JAVA_HOME=/usr/lib/jvm/java-7-oracle
```

5. Install Hadoop

```
```



[1] http://stackoverflow.com/a/17162255/323578
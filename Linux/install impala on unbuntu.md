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

sudo service impala-server start
sudo service impala-state-store start

```
3. Install java and export JAVA_HOME

```
export JAVA_HOME=/usr/lib/jvm/java-7-oracle
```

5. Install Hadoop

```
```



[1] http://stackoverflow.com/a/17162255/323578
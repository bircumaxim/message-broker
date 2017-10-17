# MessageBroker
###### Simple message broker or .NET applications build for educational purposes only. You could check bellow a short description and a guid of how to configure and to get started fast.
[![N|Solid](https://lostechies.com/derekgreer/files/2012/03/TopicExchange2.png)]()

### Building

To build MessageBroker just folow steps below:
- open `MessageBroker.sln` in Visual Studio/Rider.
- set the working directory to be `MessageBroker` directory
- select as target project `MessageBroker`
- hit the run button 

You'll find the built assemblies in /binaries.

If you see the build failing, check that you haven't put the source of MessageBroker in a deep subdirectory since long path names (greater than 248 characters) aren't supported by MSBuild.

### Configuration
MessageBroker configuration file should start with a `Broker` tag in order to configure place and format of data that will be persisted by broker server you should ad a `Persistence` tag which will have inner tags for each types of persistence for now we have just 2 types of persistence  `InMemoryPersistence` which is present by default and `FilePersistence` that you could add as presented in example below.

##### `FilePersistence`
- RootDirectory : [`"WorkingDirectory/messages" is by default`]
- EnableCrypting : [`false default`,`true`]

In order to make broker server to listen for connections and to send messages to consumers you should add a `ConnectionManagers` tag which will hold different types of connection managers. For now we have only 2 connection managers `TcpConnectionManager` and `UdpConnectionManager` as presented in example below. 

>You could add multiple ConnectionManagers of the same type just ensure that you pass different ports

##### `TcpConnectionManager`
- Port : `required` [`9000 default`]
- WireProtocol : [`DefaultWireProtocol default`]
- MaxMessageLength : [`52428800 default`]
- EnableCrypting : [`false default`,`true`]

##### `UdpConnectionManager`
- Port : `required` [`9000 default`]
- WireProtocol : [`DefaultWireProtocol default`]
- MaxMessageLength : [`52428800 default`]
- EnableCrypting : [`false default`,`true`]

##### `Exchange`
- Name : `required`,`unique`
- Types: `DirectExchange`, `TopicExchange`, `FanoutExchange`
As Inner tags each exchnage could have multiple `Queue` tags

##### `Queue`
- Name : `required`,`unique per exchange`

```xml
<?xml version="1.0" encoding="utf-8"?>`
<Broker>
    <Persistence>
        <FilePersistence RootDirectory="myDir/messages" EnableCrypting="true"/>
    </Persistence>

    <ConnectionManagers>
        <TcpConnectionManager Port="6000"
                              WireProtocol="DefaultWireProtocol"
                              MaxMessageLength="34000"
                              EnableCrypting="true"/>
        <UdpConnectionManager Port="8000"
                              WireProtocol="DefaultWireProtocol"
                              MaxMessageLength="20000"
                              EnableCrypting="false"/>
    </ConnectionManagers>

    <Exchanges>
        <DirectExchange Name="DirrectExchange">
            <Queue Name="Queue1"/>
            <Queue Name="Queue2"/>
            <Queue Name="Queue3"/>
        </DirectExchange>

        <TopicExchange Name="TopicExchange">
            <Queue Name="Queue2"/>
            <Queue Name="Queue1"/>
            <Queue Name="Queue3"/>
        </TopicExchange>

        <FanoutExchange Name="FanoutExchange">
            <Queue Name="Queue2"/>
            <Queue Name="Queue3"/>
        </FanoutExchange>
    </Exchanges>
</Broker>
```


# MessageBuss
### Building
To use message buss in your Consumers or Producers you need firstly to add MessageBuss as dependency to your projects and for this folow steps below: 
 - open `MessageBroker.sln` in Visual Studio/Rider.
 - select `MessageBuss` project and build it.
 - after you did this you could close `MessageBroker.sln` and open your sln
 - go to your `Consumer` or `Producer` project and add `MessageBuss.dll` in references
 - you could also add `Messages.dll` and `Serialization.dll` but for now they are optional maybe you could need them in near feature

### Configuration
MessageBuss configuration file should start with a `Buss` tag
In order to connect to a `ConnectionManger` that you added to MessageBroker config you should describe it in MessageBuss Config you could do this by adding a `Broker` tag

##### `Broker`
- Name : `required`, `unique`
- SocketProtocol : [`Tcp default`, `Udp`]
- Ip : `required`
- Port : `required` [`9000 default`]
- WireProtocol : [`DefaultWireProtocol default`]
- EnableCrypting : [`false default`,`true`]
- Inner tags: [`Fanout`, `Direct`, `Topic`] 
 Each of of Broker inner tags should have specifyed a exchnage `Name` according to MessageBroker config.

```xml
<?xml version="1.0" encoding="utf-8"?>`
<Buss>
    <Brokers>
        <Broker Name="Broker"
                 SocketProtocol="Tcp"
                 Ip="127.0.0.1"
                 Port="9000"
                 WireProtocol="DefaultWireProtocol"
                 EnableCrypting="true">

            <Fanout Name="DefaultFanoutExchange"/>
            <Direct Name="DefaultDirectExchange"/>
            <Topic Name="DefaultTopicExchange"/>
        </Broker>

        <Broker Name="Broker2"
                 SocketProtocol="Udp"
                 Ip="127.0.0.1"
                 Port="8000"
                 WireProtocol="DefaultWireProtocol">

            <Fanout Name="DefaultFanoutExchange"/>
            <Direct Name="DefaultDirectExchange"/>
            <Topic Name="DefaultTopicExchange"/>
        </Broker>
    </Brokers>
</Buss>
```

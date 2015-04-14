#Raft Consensus Visualization & CaliDiagram

######This repository was supposed to be be only for CaliDiagram library but it slowly evelves into visualization of any actor system. Current goal is to provide solution that would help developing and visualizing new distributed algorithms more easily.

***
#####Projects incuded in solution:
 * CaliDiagram - library for WPF diagramming that uses ViewModel-First approach,
 * NetworkModel - framework for messaging, now InProc network simulation only,
 * RaftAlgorithm - Buggy and incomplete implementation of Raft Consensus algorithm,
 * RaftDemo - WPF application for visualizing raft algorithm, needs to be separated into two projects.

#####What needs to be done:
 * Improve network model so it could emulate tcp/ip like messaging correctly,
 * Figure out basic elements of actor system(ClientActor, ServerActor,etc),
 * Wire up network model to GUI for connection/socket/packet visualization,
 * Separate RaftVisualization as an example use of code from this repo,
 * Finally, make it possible to wire netowrk model to any existing messaging library, possibly NetMQ.

#####CaliDiagram features:
* Creating your own diagram nodes using ViewModel first approach
* Customizing connections between nodes
* Bezier/straight connection line
* Customizing attach descriptors
* Saving and loading to XML
 
#####Some progress:
![raft2](https://cloud.githubusercontent.com/assets/3065454/7144999/7ec540f0-e2e9-11e4-9346-f0d540c409b5.png)


![sample diagram](http://95.85.14.120/dwn/cali.png)


![sample diagram](https://cloud.githubusercontent.com/assets/3065454/5565103/b83c2e1e-8ee5-11e4-803a-992050d5fa0c.png)

![calidiagram view-model first](https://cloud.githubusercontent.com/assets/3065454/5696991/2711f360-99e4-11e4-808a-8a221aafcfd6.png)

package sk.ikim23.carrental.core.impl

interface IStats {
    fun systemTime(): Double
    fun customerCount(): Int
    fun avgSysTime(): Double
    fun roundCount(): Int
    fun lowSysTime(): Double
    fun avgRoundTime(): Double
    fun uppSysTime(): Double
    fun avgBusUsage(): Double
    fun avgT1QueueSize(): Double
    fun avgT2QueueSize(): Double
    fun avgServiceDeskQueueSize(): Double
    fun avgServiceDeskUsage(): Double
    fun clear()

    fun print() {
        println("System time: ${systemTime()}")
        println("Average user time: ${avgSysTime()}")
        println("Average bus round time: ${avgRoundTime()}")
        println("Average bus usage: ${avgBusUsage()}")
        println("Average T1 queue: ${avgT1QueueSize()}")
        println("Average T2 queue: ${avgT2QueueSize()}")
        println("Average service desk queue: ${avgServiceDeskQueueSize()}")
        println("Average service desk usage: ${avgServiceDeskUsage()}")
    }
}
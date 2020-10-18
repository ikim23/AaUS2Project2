package sk.ikim23.carrental.core.impl

import sk.ikim23.carrental.core.obj.Bus
import sk.ikim23.carrental.core.obj.Customer
import sk.ikim23.carrental.safeDiv

class Stats(val core: SimCore) : IStats {
    var nBuses = 0
    var nCustomers = 0
    var sumTime = 0.0
    var sumTimeSquare = 0.0
    var sumRoundTime = 0.0
    var sumBusUsage = 0.0

    fun take(customer: Customer) {
        val time = customer.serviceTime - customer.arrivalTime
        sumTime += time
        sumTimeSquare += time * time
        nCustomers++
    }

    fun take(bus: Bus) {
        val roundTime = core.currentTime - bus.departure
        bus.departure = core.currentTime
        if (roundTime > 0) {
            sumRoundTime += roundTime
            sumBusUsage += bus.usedCapacity()
            nBuses++
        }
    }

    override fun systemTime() = core.currentTime
    override fun customerCount() = nCustomers
    override fun lowSysTime(): Double {
        val w = sumTime / nCustomers
        val s = Math.sqrt(sumTimeSquare / nCustomers - w * w)
        val avg = sumTime / nCustomers
        return avg - 1.96 * s / Math.sqrt((nCustomers - 1).toDouble()) safeDiv 60
    }

    override fun uppSysTime(): Double {
        val w = sumTime / nCustomers
        val s = Math.sqrt(sumTimeSquare / nCustomers - w * w)
        val avg = sumTime / nCustomers
        return avg + 1.96 * s / Math.sqrt((nCustomers - 1).toDouble()) safeDiv 60
    }

    override fun avgSysTime() = (sumTime / nCustomers) safeDiv 60
    override fun roundCount() = nBuses
    override fun avgRoundTime() = (sumRoundTime / nBuses) safeDiv 60
    override fun avgBusUsage() = sumBusUsage safeDiv nBuses
    override fun avgT1QueueSize() = core.t1Queue.averageSize()
    override fun avgT2QueueSize() = core.t2Queue.averageSize()
    override fun avgServiceDeskQueueSize() = core.serviceDesk.averageSize()
    override fun avgServiceDeskUsage() = core.serviceDesk.averageUsage()

    override fun clear() {
        nBuses = 0
        nCustomers = 0
        sumTime = 0.0
        sumRoundTime = 0.0
        sumBusUsage = 0.0
    }
}
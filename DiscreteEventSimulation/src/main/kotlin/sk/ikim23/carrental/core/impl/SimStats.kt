package sk.ikim23.carrental.core.impl

import sk.ikim23.carrental.core.obj.Bus
import sk.ikim23.carrental.core.obj.Customer
import sk.ikim23.carrental.safeDiv

class SimStats(private val core: SimCore) {
    private var nBuses = 0
    private var nCustomers = 0
    private var sumCustomerTime = 0.0
    private var sumCustomerTimeSquare = 0.0
    private var sumBusRoundTime = 0.0
    private var sumBusUsage = 0.0

    fun take(customer: Customer) {
        val time = customer.serviceTime - customer.arrivalTime
        sumCustomerTime += time
        sumCustomerTimeSquare += time * time
        nCustomers++
    }

    fun take(bus: Bus) {
        val roundTime = core.currentTime - bus.departure
        bus.departure = core.currentTime
        if (roundTime > 0) {
            sumBusRoundTime += roundTime
            sumBusUsage += bus.usedCapacity()
            nBuses++
        }
    }

    fun systemTime() = core.currentTime
    fun customerCount() = nCustomers
    fun lowSysTime(): Double {
        val avg = sumCustomerTime / nCustomers
        val s = Math.sqrt(sumCustomerTimeSquare / nCustomers - avg * avg)
        return (avg - 1.96 * s / Math.sqrt((nCustomers - 1).toDouble())) safeDiv 60
    }

    fun uppSysTime(): Double {
        val avg = sumCustomerTime / nCustomers
        val s = Math.sqrt(sumCustomerTimeSquare / nCustomers - avg * avg)
        return (avg + 1.96 * s / Math.sqrt((nCustomers - 1).toDouble())) safeDiv 60
    }

    fun avgSysTime() = (sumCustomerTime / nCustomers) safeDiv 60
    fun roundCount() = nBuses
    fun avgRoundTime() = (sumBusRoundTime / nBuses) safeDiv 60
    fun avgBusUsage() = sumBusUsage safeDiv nBuses
    fun avgT1QueueSize() = core.t1Queue.averageSize()
    fun avgT2QueueSize() = core.t2Queue.averageSize()
    fun avgServiceDeskQueueSize() = core.serviceDesk.averageSize()
    fun avgServiceDeskUsage() = core.serviceDesk.averageUsage()

    fun clear() {
        nBuses = 0
        nCustomers = 0
        sumCustomerTime = 0.0
        sumCustomerTimeSquare = 0.0
        sumBusRoundTime = 0.0
        sumBusUsage = 0.0
    }

    class GlobalStats {
        fun a() {

        }

    }
}
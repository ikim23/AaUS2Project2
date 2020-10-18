package sk.ikim23.carrental.core.obj

import sk.ikim23.carrental.core.ITime
import sk.ikim23.carrental.safeDiv
import java.util.*

class ServiceDesk(val time: ITime, val capacity: Int) {
    private val servedCustomers = StatsQueue<Customer>(time)
    private val deskNumbers = LinkedList<Int>()

    init {
        deskNumbers += 1..capacity
    }

    fun isFull() = servedCustomers.size() >= capacity

    fun add(customer: Customer) {
        if (isFull()) throw IllegalArgumentException("${javaClass.simpleName} is already full")
        customer.serviceDeskNumber = deskNumbers.poll()
        servedCustomers.add(customer)
    }

    fun remove(customer: Customer) {
        if (!servedCustomers.remove(customer)) throw IllegalStateException("$customer was not found")
        customer.serviceTime = time.currentTime
        deskNumbers.add(customer.serviceDeskNumber)
    }

    fun clear() = servedCustomers.clear()

    fun averageSize() = servedCustomers.averageSize()

    fun averageUsage() = averageSize() safeDiv capacity

    override fun toString(): String {
        val sb = StringBuilder("ServiceDesk(servedCustomers=${servedCustomers.size()}):\n")
        for (c in servedCustomers) {
            sb.append("${c.serviceDeskNumber} is serving $c\n")
        }
        for (n in deskNumbers) {
            sb.append("$n is free\n")
        }
        return sb.toString()
    }
}
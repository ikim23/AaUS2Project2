package sk.ikim23.carrental.core.obj

import java.util.*

class Bus(val capacity: Int = 12) {
    companion object {
        var ID = 1
    }

    private val passengers: Queue<Customer> = LinkedList()
    val id = ID++
    var departure = Double.MAX_VALUE
    var destination = ""

    fun isFull() = passengers.size >= capacity

    fun isEmpty() = passengers.isEmpty()

    fun add(customer: Customer) {
        if (isFull()) throw IllegalStateException("$this is already full")
        passengers.add(customer)
    }

    fun remove(): Customer = passengers.remove()

    fun usedCapacity() = passengers.size.toDouble() / capacity

    override fun toString() = "Bus(id=$id, usedCapacity=${passengers.size}) on the way to $destination"
}
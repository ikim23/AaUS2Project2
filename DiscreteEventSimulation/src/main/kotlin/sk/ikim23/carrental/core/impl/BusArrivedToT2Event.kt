package sk.ikim23.carrental.core.impl

import sk.ikim23.carrental.core.Event
import sk.ikim23.carrental.core.obj.Bus

class BusArrivedToT2Event(val core: SimCore, val bus: Bus, execTime: Double? = null)
    : Event(core, execTime ?: core.currentTime + core.tTimeToT2) {
    init {
        bus.destination = "T2"
    }

    override fun exec() {
        log("$bus arrived to T2")
        if (!bus.isFull() && !core.t2Queue.isEmpty()) {
            // load passenger
            val customer = core.t2Queue.remove()
            val loadCustomer = BusLoadsCstOnT2Event(core, bus, customer)
            core.addEvent(loadCustomer)
        } else {
            // leave without loading (full or empty queue)
            val rideToRental = BusArrivedToRentalEvent(core, bus)
            core.addEvent(rideToRental)
        }
    }
}
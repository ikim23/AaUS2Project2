package sk.ikim23.carrental.core.impl

import sk.ikim23.carrental.core.Event
import sk.ikim23.carrental.core.obj.Bus

class BusArrivedToT1Event(val core: SimCore, val bus: Bus, execTime: Double? = null)
    : Event(core, execTime ?: core.currentTime + core.tTimeToT1) {
    init {
        bus.destination = "T1"
    }

    override fun exec() {
        log("$bus arrived to T1")
        if (!bus.isFull() && !core.t1Queue.isEmpty()) {
            // load passengers
            val customer = core.t1Queue.remove()
            val loadCustomer = BusLoadsCstOnT1Event(core, bus, customer)
            core.addEvent(loadCustomer)
        } else {
            // leave empty
            val rideToT2 = BusArrivedToT2Event(core, bus)
            core.addEvent(rideToT2)
        }
    }
}
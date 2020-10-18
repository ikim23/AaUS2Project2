package sk.ikim23.carrental.core.impl

import sk.ikim23.carrental.core.Event
import sk.ikim23.carrental.core.obj.Bus
import sk.ikim23.carrental.core.obj.Customer

class BusLoadsCstOnT2Event(val core: SimCore, val bus: Bus, val customer: Customer) : Event(core, core.currentTime + core.rLoadOnT2.nextDouble()) {
    override fun exec() {
        bus.add(customer)
        log("$customer loaded on $bus")
        if (!bus.isFull() && !core.t2Queue.isEmpty()) {
            // load passenger
            val nextCustomer = core.t2Queue.remove()
            val nextLoad = BusLoadsCstOnT2Event(core, bus, nextCustomer)
            core.addEvent(nextLoad)
        } else {
            // bus leaves T2
            val rideToRental = BusArrivedToRentalEvent(core, bus)
            core.addEvent(rideToRental)
        }
    }
}
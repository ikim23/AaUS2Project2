package sk.ikim23.carrental.core.impl

import sk.ikim23.carrental.core.Event
import sk.ikim23.carrental.core.obj.Bus
import sk.ikim23.carrental.core.obj.Customer

class BusLoadsCstOnT1Event(val core: SimCore, val bus: Bus, val customer: Customer) : Event(core, core.currentTime + core.rLoadOnT1.nextDouble()) {
    override fun exec() {
        bus.add(customer)
        log("$customer loaded on $bus")
        if (!bus.isFull() && !core.t1Queue.isEmpty()) {
            // load passenger
            val nextCustomer = core.t1Queue.remove()
            val nextLoad = BusLoadsCstOnT1Event(core, bus, nextCustomer)
            core.addEvent(nextLoad)
        } else {
            // bus leaves T1
            val rideToT2 = BusArrivedToT2Event(core, bus)
            core.addEvent(rideToT2)
        }
    }
}
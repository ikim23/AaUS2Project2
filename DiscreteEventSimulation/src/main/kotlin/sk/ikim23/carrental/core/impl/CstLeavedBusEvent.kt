package sk.ikim23.carrental.core.impl

import sk.ikim23.carrental.core.Event
import sk.ikim23.carrental.core.obj.Bus

class CstLeavedBusEvent(val core: SimCore, val bus: Bus) : Event(core, core.currentTime + core.rLeavedBus.nextDouble()) {
    override fun exec() {
        val customer = bus.remove()
        log("$customer leaved $bus")
        core.rentalQueue.add(customer)
        if (!core.serviceDesk.isFull()) {
            val servedCustomer = core.rentalQueue.remove()
            core.serviceDesk.add(servedCustomer)
            val serviceEvent = CstGotKeysEvent(core, servedCustomer)
            core.addEvent(serviceEvent)
        }
        if (!bus.isEmpty()) {
            val nextLeave = CstLeavedBusEvent(core, bus)
            core.addEvent(nextLeave)
        } else if (core.customersAreWaiting()) {
            val rideToT1 = BusArrivedToT1Event(core, bus)
            core.addEvent(rideToT1)
        }
    }
}
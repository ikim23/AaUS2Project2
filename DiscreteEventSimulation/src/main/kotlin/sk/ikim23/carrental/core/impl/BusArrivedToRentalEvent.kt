package sk.ikim23.carrental.core.impl

import sk.ikim23.carrental.core.Event
import sk.ikim23.carrental.core.obj.Bus

class BusArrivedToRentalEvent(val core: SimCore, val bus: Bus, execTime: Double? = null)
    : Event(core, execTime ?: core.currentTime + core.tTimeToRental) {
    init {
        bus.destination = "CR"
    }

    override fun exec() {
        log("$bus arrived to Rental shop")
        core.stats.take(bus)
        if (!bus.isEmpty()) {
            val nextLeave = CstLeavedBusEvent(core, bus)
            core.addEvent(nextLeave)
        } else if (core.customersAreWaiting()) {
            val rideToT1 = BusArrivedToT1Event(core, bus)
            core.addEvent(rideToT1)
        }
    }
}
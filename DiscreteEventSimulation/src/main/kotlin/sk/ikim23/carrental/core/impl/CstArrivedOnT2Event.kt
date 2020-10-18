package sk.ikim23.carrental.core.impl

import sk.ikim23.carrental.core.Event
import sk.ikim23.carrental.core.obj.Customer

class CstArrivedOnT2Event(val core: SimCore) : Event(core, core.currentTime + core.rArrivalToT2.nextDouble()) {
    override fun exec() {
        val customer = Customer(execTime)
        core.t2Queue.add(customer)
        log("$customer arrived on T2")
        if (core.hasTime()) {
            val nextArrival = CstArrivedOnT2Event(core)
            core.addEvent(nextArrival)
        }
    }
}
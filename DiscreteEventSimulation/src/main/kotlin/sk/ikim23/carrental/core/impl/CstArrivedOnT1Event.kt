package sk.ikim23.carrental.core.impl

import sk.ikim23.carrental.core.Event
import sk.ikim23.carrental.core.obj.Customer

class CstArrivedOnT1Event(val core: SimCore) : Event(core, core.currentTime + core.rArrivalToT1.nextDouble()) {
    override fun exec() {
        val customer = Customer(execTime)
        core.t1Queue.add(customer)
        log("$customer arrived on T1")
        if (core.hasTime()) {
            val nextArrival = CstArrivedOnT1Event(core)
            core.addEvent(nextArrival)
        }
    }
}
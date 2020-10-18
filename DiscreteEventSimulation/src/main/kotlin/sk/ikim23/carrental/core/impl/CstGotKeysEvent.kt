package sk.ikim23.carrental.core.impl

import sk.ikim23.carrental.core.Event
import sk.ikim23.carrental.core.obj.Customer

class CstGotKeysEvent(val core: SimCore, val customer: Customer) : Event(core, core.currentTime + core.rService.nextDouble()) {
    override fun exec() {
        log("$customer got car")
        core.serviceDesk.remove(customer)
        core.stats.take(customer)
        if (!core.rentalQueue.isEmpty()) {
            val servedCustomer = core.rentalQueue.remove()
            core.serviceDesk.add(servedCustomer)
            val serviceEvent = CstGotKeysEvent(core, servedCustomer)
            core.addEvent(serviceEvent)
        }
    }
}
package sk.ikim23.aircarrental.message

import OSPABA.MessageForm
import OSPABA.Simulation
import sk.ikim23.aircarrental.M
import sk.ikim23.aircarrental.entity.Bus
import sk.ikim23.aircarrental.entity.Passenger
import sk.ikim23.aircarrental.entity.ServiceDesk

class MyMessage : MessageForm {
    val id = ++ID
    var bus: Bus? = null
    var passenger: Passenger? = null
    var worker: ServiceDesk? = null

    constructor(mySim: Simulation, code: Int = -1, addressee: Int = -1, bus: Bus? = null, passenger: Passenger? = null, worker: ServiceDesk? = null)
            : super(mySim) {
        setCode(code)
        setAddressee(addressee)
        this.bus = bus
        this.passenger = passenger
        this.worker = worker
    }

    constructor(other: MessageForm) : super(other) {
        if (other is MyMessage) {
            bus = other.bus
            passenger = other.passenger
            worker = other.worker
        }
    }

    override fun createCopy() = MyMessage(this)

    fun withCode(code: Int): MyMessage {
        setCode(code)
        return this
    }

    fun withAddressee(addressee: Int): MyMessage {
        setAddressee(addressee)
        return this
    }

    override fun toString(): String {
        return "MyMessage(time=${deliveryTime()}, id=$id, code=${M.getMessageName(code())}, sender=${sender()?.javaClass?.simpleName}, addressee=${addressee()?.javaClass?.simpleName})"
    }

    companion object {
        var ID = 0
    }
}
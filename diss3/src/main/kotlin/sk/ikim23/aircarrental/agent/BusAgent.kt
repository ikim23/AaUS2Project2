package sk.ikim23.aircarrental.agent

import OSPABA.Agent
import sk.ikim23.aircarrental.M
import sk.ikim23.aircarrental.assistant.MoveBusAssistant
import sk.ikim23.aircarrental.entity.Bus
import sk.ikim23.aircarrental.entity.BusType
import sk.ikim23.aircarrental.manager.BusManager
import sk.ikim23.aircarrental.sim.*

class BusAgent(mySim: MySimulation, parent: Agent) : ResettableAgent(ID.AGE_BUS, mySim, parent) {
    var buses = arrayOf<Bus>()

    init {
        BusManager(mySim, this)
        MoveBusAssistant(mySim, this)
        addOwnMessage(M.INIT)
        addOwnMessage(M.MOVE_BUS)
        addOwnMessage(M.MOVE_BUS_DONE)
    }

    fun init(nBuses: Int, type: BusType) {
        buses = Array(nBuses) { Bus(it + 1, type, mySim()) }
    }

    override fun reset() {
        super.reset()
        buses.forEach { it.reset() }
    }
}
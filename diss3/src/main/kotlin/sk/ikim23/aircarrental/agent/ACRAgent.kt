package sk.ikim23.aircarrental.agent

import OSPABA.Agent
import OSPABA.Simulation
import sk.ikim23.aircarrental.sim.ID
import sk.ikim23.aircarrental.M
import sk.ikim23.aircarrental.manager.ACRManager
import sk.ikim23.aircarrental.sim.ResettableAgent

class ACRAgent(mySim: Simulation, parent: Agent) : ResettableAgent(ID.AGE_ACR, mySim, parent) {
    init {
        ACRManager(mySim, this)
        addOwnMessage(M.MOVE_BUS_DONE)
        addOwnMessage(M.MOVE_BUS_TO_NEXT_DESTINATION)
    }
}
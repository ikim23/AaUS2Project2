package sk.ikim23.aircarrental.agent

import OSPABA.Agent
import OSPABA.Simulation
import sk.ikim23.aircarrental.sim.ID
import sk.ikim23.aircarrental.M
import sk.ikim23.aircarrental.assistant.TakeOffBusSharedAssistant
import sk.ikim23.aircarrental.manager.DepartureTerminalManager
import sk.ikim23.aircarrental.sim.ResettableAgent

class DepartureTerminalAgent(mySim: Simulation, parent: Agent) : ResettableAgent(ID.AGE_DEPARTURE_TERMINAL, mySim, parent) {
    init {
        DepartureTerminalManager(mySim, this)
        addOwnMessage(M.BUS_ARRIVED)

        TakeOffBusSharedAssistant(mySim, this, ID.ASS_TAKE_OFF_BUS_AT_DEPARTURE_TERMINAL)
        addOwnMessage(M.TAKE_OFF_FROM_BUS)
        addOwnMessage(M.TAKE_OFF_FROM_BUS_DONE)
        addOwnMessage(M.PROCESS_PASSENGER_FROM_BUS)
        addOwnMessage(M.BUS_EMPTIED)
    }
}
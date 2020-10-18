package sk.ikim23.aircarrental.agent

import OSPABA.Agent
import OSPABA.Simulation
import sk.ikim23.aircarrental.sim.ID
import sk.ikim23.aircarrental.M
import sk.ikim23.aircarrental.assistant.ILoadOnBusAssistantMaster
import sk.ikim23.aircarrental.assistant.LoadOnBusSharedAssistant
import sk.ikim23.aircarrental.entity.Bus
import sk.ikim23.aircarrental.entity.Passenger
import sk.ikim23.aircarrental.entity.PassengerQueue
import sk.ikim23.aircarrental.entity.Place
import sk.ikim23.aircarrental.manager.ArrivalTerminalManager
import sk.ikim23.aircarrental.sim.ResettableAgent

class ArrivalTerminalAgent(mySim: Simulation, parent: Agent) : ResettableAgent(ID.AGE_ARRIVAL_TERMINAL, mySim, parent), ILoadOnBusAssistantMaster {
    val terminals = mapOf(
            Place.T1 to PassengerQueue(mySim),
            Place.T2 to PassengerQueue(mySim)
    )

    init {
        ArrivalTerminalManager(mySim, this)
        addOwnMessage(M.BUS_ARRIVED)
        addOwnMessage(M.PASSENGER_ARRIVED)

        LoadOnBusSharedAssistant(mySim, this, ID.ASS_LOAD_ON_BUS_AT_ARRIVAL_TERMINAL, this)
        addOwnMessage(M.LOAD_PASSENGER)
        addOwnMessage(M.LOAD_PASSENGER_DONE)
        addOwnMessage(M.BUS_LOADED)
    }

    fun queue(passenger: Passenger) {
        val queue = terminals[passenger.arrivalPlace]
        if (queue == null) {
            throw IllegalStateException("Terminal ${passenger.arrivalPlace} is not initialized")
        }
        queue.add(passenger)
    }

    override fun nextPassenger(bus: Bus?): Passenger? {
        if (bus == null) {
            return null
        }
        val queue = terminals[bus.destination]
        val passenger = queue?.popMaxSize(bus.freeSeats())
        return passenger
    }

    override fun reset() {
        super.reset()
        terminals.values.forEach { it.reset() }
    }
}

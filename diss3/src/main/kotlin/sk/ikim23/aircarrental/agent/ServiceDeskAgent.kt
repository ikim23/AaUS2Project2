package sk.ikim23.aircarrental.agent

import OSPABA.Agent
import OSPABA.Simulation
import sk.ikim23.aircarrental.sim.ID
import sk.ikim23.aircarrental.M
import sk.ikim23.aircarrental.assistant.*
import sk.ikim23.aircarrental.entity.Bus
import sk.ikim23.aircarrental.entity.Passenger
import sk.ikim23.aircarrental.entity.PassengerQueue
import sk.ikim23.aircarrental.entity.ServiceDesk
import sk.ikim23.aircarrental.manager.ServiceDeskManager
import sk.ikim23.aircarrental.sim.ResettableAgent

class ServiceDeskAgent(mySim: Simulation, parent: Agent) : ResettableAgent(ID.AGE_SERVICE_DESK, mySim, parent), ILoadOnBusAssistantMaster {
    val qToService = PassengerQueue(mySim)
    val qToT3 = PassengerQueue(mySim)
    var workers = arrayOf<ServiceDesk>()

    init {
        ServiceDeskManager(mySim, this)
        addOwnMessage(M.PASSENGER_ARRIVED)
        addOwnMessage(M.BUS_ARRIVED)

        TakeOffBusSharedAssistant(mySim, this, ID.ASS_TAKE_OFF_BUS_AT_SERVICE_DESK)
        addOwnMessage(M.TAKE_OFF_FROM_BUS)
        addOwnMessage(M.TAKE_OFF_FROM_BUS_DONE)
        addOwnMessage(M.PROCESS_PASSENGER_FROM_BUS)
        addOwnMessage(M.BUS_EMPTIED)

        ServePassengerAssistant(mySim, this)
        addOwnMessage(M.SERVE_PASSENGER)
        addOwnMessage(M.SERVE_PASSENGER_DONE)
        addOwnMessage(M.QUEUE_PASSENGER_TO_T3)
        addOwnMessage(M.PASSENGER_LEAVES_SYSTEM)

        LoadOnBusSharedAssistant(mySim, this, ID.ASS_LOAD_ON_BUS_AT_SERVICE_DESK, this)
        addOwnMessage(M.LOAD_PASSENGER)
        addOwnMessage(M.LOAD_PASSENGER_DONE)
        addOwnMessage(M.BUS_LOADED)
    }

    fun init(nEmployees: Int) {
        workers = Array(nEmployees) { ServiceDesk(it + 1, mySim()) }
    }

    override fun nextPassenger(bus: Bus?): Passenger? {
        if (bus == null) {
            return null
        }
        val passenger = qToT3.popMaxSize(bus.freeSeats())
        return passenger
    }

    override fun reset() {
        super.reset()
        qToService.reset()
        qToT3.reset()
        workers.forEach { it.reset() }
    }
}
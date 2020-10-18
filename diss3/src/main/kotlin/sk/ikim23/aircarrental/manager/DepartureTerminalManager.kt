package sk.ikim23.aircarrental.manager

import OSPABA.Agent
import OSPABA.Manager
import OSPABA.MessageForm
import OSPABA.Simulation
import sk.ikim23.aircarrental.*
import sk.ikim23.aircarrental.message.MyMessage
import sk.ikim23.aircarrental.sim.ID
import sk.ikim23.aircarrental.sim.MessageNotImplementedException
import sk.ikim23.aircarrental.sim.withCast

class DepartureTerminalManager(mySim: Simulation, myAgent: Agent) : Manager(ID.MNG_DEPARTURE_TERMINAL, mySim, myAgent) {
    override fun processMessage(messageForm: MessageForm) {
        withCast(this, messageForm) { msg: MyMessage ->
            when (msg.code()) {
                M.BUS_ARRIVED -> {
                    notice(msg.withCode(M.TAKE_OFF_FROM_BUS).withAddressee(ID.ASS_TAKE_OFF_BUS_AT_DEPARTURE_TERMINAL))
                }
                M.PROCESS_PASSENGER_FROM_BUS -> {
                    notice(msg.withCode(M.PASSENGER_LEAVES_SYSTEM).withAddressee(ID.AGE_OUTSIDE))
                }
                M.BUS_EMPTIED -> {
                    notice(msg.withCode(M.MOVE_BUS_TO_NEXT_DESTINATION).withAddressee(ID.AGE_ACR))
                }
                else -> throw MessageNotImplementedException(msg, this)
            }
        }
    }
}
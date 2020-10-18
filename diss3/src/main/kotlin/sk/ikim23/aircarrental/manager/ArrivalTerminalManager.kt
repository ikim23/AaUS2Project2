package sk.ikim23.aircarrental.manager

import OSPABA.Manager
import OSPABA.MessageForm
import OSPABA.Simulation
import sk.ikim23.aircarrental.*
import sk.ikim23.aircarrental.agent.ArrivalTerminalAgent
import sk.ikim23.aircarrental.message.MyMessage
import sk.ikim23.aircarrental.sim.ID
import sk.ikim23.aircarrental.sim.MessageNotImplementedException
import sk.ikim23.aircarrental.sim.withCast

class ArrivalTerminalManager(mySim: Simulation, val myAgent: ArrivalTerminalAgent) : Manager(ID.MNG_ARRIVAL_TERMINAL, mySim, myAgent) {
    override fun processMessage(messageForm: MessageForm) {
        withCast(this, messageForm) { msg: MyMessage ->
            when (msg.code()) {
                M.BUS_ARRIVED -> {
                    notice(msg.createCopy().withCode(M.LOAD_PASSENGER).withAddressee(ID.ASS_LOAD_ON_BUS_AT_ARRIVAL_TERMINAL))
                }
                M.PASSENGER_ARRIVED -> {
                    myAgent.queue(msg.passenger!!)
                }
                M.BUS_LOADED -> {
                    notice(msg.withCode(M.MOVE_BUS_TO_NEXT_DESTINATION).withAddressee(ID.AGE_ACR))
                }
                else -> throw MessageNotImplementedException(msg, this)
            }
        }
    }
}
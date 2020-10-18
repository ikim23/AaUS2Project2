package sk.ikim23.aircarrental.manager

import OSPABA.Manager
import OSPABA.MessageForm
import OSPABA.Simulation
import sk.ikim23.aircarrental.*
import sk.ikim23.aircarrental.agent.BusAgent
import sk.ikim23.aircarrental.message.MyMessage
import sk.ikim23.aircarrental.sim.ID
import sk.ikim23.aircarrental.sim.MessageNotImplementedException
import sk.ikim23.aircarrental.sim.withCast

class BusManager(mySim: Simulation, val myAgent: BusAgent) : Manager(ID.MNG_BUS, mySim, myAgent) {
    override fun processMessage(messageForm: MessageForm) {
        withCast(this, messageForm) { msg: MyMessage ->
            when (msg.code()) {
                M.INIT, M.MOVE_BUS -> {
                    notice(msg.withAddressee(ID.ASS_MOVE_BUS))
                }
                M.MOVE_BUS_DONE -> {
                    notice(msg.withAddressee(ID.AGE_ACR))
                }
                else -> throw MessageNotImplementedException(msg, this)
            }
        }
    }
}
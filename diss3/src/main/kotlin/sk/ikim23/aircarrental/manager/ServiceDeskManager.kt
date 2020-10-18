package sk.ikim23.aircarrental.manager

import OSPABA.Manager
import OSPABA.MessageForm
import OSPABA.Simulation
import sk.ikim23.aircarrental.*
import sk.ikim23.aircarrental.agent.ServiceDeskAgent
import sk.ikim23.aircarrental.message.MyMessage
import sk.ikim23.aircarrental.sim.ID
import sk.ikim23.aircarrental.sim.MessageNotImplementedException
import sk.ikim23.aircarrental.sim.withCast

class ServiceDeskManager(mySim: Simulation, val myAgent: ServiceDeskAgent) : Manager(ID.MNG_SERVICE_DESK, mySim, myAgent) {
    override fun processMessage(messageForm: MessageForm) {
        withCast(this, messageForm) { msg: MyMessage ->
            when (msg.code()) {
                M.BUS_ARRIVED -> {
                    notice(msg.withCode(M.TAKE_OFF_FROM_BUS).withAddressee(ID.ASS_TAKE_OFF_BUS_AT_SERVICE_DESK))
                }
                M.PASSENGER_ARRIVED, M.PROCESS_PASSENGER_FROM_BUS -> {
                    myAgent.qToService.add(msg.passenger)
                    notice(MyMessage(mySim(), M.SERVE_PASSENGER, ID.ASS_SERVE_PASSENGER))
                }
                M.BUS_EMPTIED -> {
                    notice(msg.withCode(M.LOAD_PASSENGER).withAddressee(ID.ASS_LOAD_ON_BUS_AT_SERVICE_DESK))
                }
                M.QUEUE_PASSENGER_TO_T3 -> {
                    myAgent.qToT3.add(msg.passenger)
                }
                M.PASSENGER_LEAVES_SYSTEM -> {
                    notice(msg.withAddressee(ID.AGE_OUTSIDE))
                }
                M.BUS_LOADED -> {
                    notice(msg.withCode(M.MOVE_BUS_TO_NEXT_DESTINATION).withAddressee(ID.AGE_ACR))
                }
                else -> throw MessageNotImplementedException(msg, this)
            }
        }
    }
}
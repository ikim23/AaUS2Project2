package sk.ikim23.aircarrental.manager

import OSPABA.Agent
import OSPABA.Manager
import OSPABA.MessageForm
import OSPABA.Simulation
import sk.ikim23.aircarrental.*
import sk.ikim23.aircarrental.entity.Place
import sk.ikim23.aircarrental.message.MyMessage
import sk.ikim23.aircarrental.sim.ID
import sk.ikim23.aircarrental.sim.MessageNotImplementedException
import sk.ikim23.aircarrental.sim.withCast

class ACRManager(mySim: Simulation, myAgent: Agent) : Manager(ID.MNG_ACR, mySim, myAgent) {
    override fun processMessage(messageForm: MessageForm) {
        withCast(this, messageForm) { msg: MyMessage ->
            when (messageForm.code()) {
                M.MOVE_BUS_DONE -> {
                    msg.setCode(M.BUS_ARRIVED)
                    when (msg.bus?.destination) {
                        Place.T1, Place.T2 -> {
                            notice(msg.withAddressee(ID.AGE_ARRIVAL_TERMINAL))
                        }
                        Place.T3 -> {
                            notice(msg.withAddressee(ID.AGE_DEPARTURE_TERMINAL))
                        }
                        Place.SERVICE_DESK -> {
                            notice(msg.withAddressee(ID.AGE_SERVICE_DESK))
                        }
                    }
                }
                M.MOVE_BUS_TO_NEXT_DESTINATION -> {
                    request(msg.withCode(M.MOVE_BUS).withAddressee(ID.AGE_BUS))
                }
                else -> throw MessageNotImplementedException(msg, this)
            }
        }
    }
}
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

class BossManager(mySim: Simulation, myAgent: Agent) : Manager(ID.MNG_BOSS, mySim, myAgent) {
    override fun processMessage(messageForm: MessageForm) {
        withCast(this, messageForm) { msg: MyMessage ->
            when (messageForm.code()) {
                M.INIT -> {
                    notice(MyMessage(mySim(), M.INIT, ID.AGE_BUS))
                    notice(MyMessage(mySim(), M.INIT, ID.AGE_OUTSIDE))
                }
                M.PASSENGER_ARRIVED -> {
                    when (msg.passenger?.arrivalPlace) {
                        Place.T1, Place.T2 -> {
                            notice(msg.withAddressee(ID.AGE_ARRIVAL_TERMINAL))
                        }
                        Place.SERVICE_DESK -> {
                            notice(msg.withAddressee(ID.AGE_SERVICE_DESK))
                        }
                        else -> throw MessageNotImplementedException(msg, this)
                    }

                }
                else -> throw MessageNotImplementedException(msg, this)
            }
        }
    }
}
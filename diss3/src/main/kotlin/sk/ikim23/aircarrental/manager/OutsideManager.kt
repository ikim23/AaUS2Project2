package sk.ikim23.aircarrental.manager

import OSPABA.*
import sk.ikim23.aircarrental.*
import sk.ikim23.aircarrental.agent.OutsideAgent
import sk.ikim23.aircarrental.entity.Place
import sk.ikim23.aircarrental.message.MyMessage
import sk.ikim23.aircarrental.sim.ID
import sk.ikim23.aircarrental.sim.MessageNotImplementedException
import sk.ikim23.aircarrental.sim.withCast

class OutsideManager(mySim: Simulation, val myAgent: OutsideAgent) : Manager(ID.MNG_OUTSIDE, mySim, myAgent) {
    override fun processMessage(messageForm: MessageForm) {
        withCast(this, messageForm) { msg: MyMessage ->
            when (messageForm.code()) {
                M.INIT -> {
                    notice(MyMessage(mySim(), M.INIT, ID.ASS_T1_ARRIVAL))
                    notice(MyMessage(mySim(), M.INIT, ID.ASS_T2_ARRIVAL))
                    notice(MyMessage(mySim(), M.INIT, ID.ASS_SERVICE_DESK_ARRIVAL))
                }
                M.PASSENGER_ARRIVED -> {
                    val passenger = msg.passenger
                    if (passenger == null) {
                        throw IllegalStateException()
                    }
                    myAgent.totalGenerated++
                    when (passenger.arrivalPlace) {
                        Place.T1 -> myAgent.t1PassengerCount.inc(passenger.size)
                        Place.T2 -> myAgent.t2PassengerCount.inc(passenger.size)
                        Place.SERVICE_DESK -> myAgent.serviceDeskPassengerCount.inc(passenger.size)
                        else -> throw MessageNotImplementedException(msg, this)
                    }
                    notice(msg.withAddressee(ID.AGE_BOSS))
                }
                M.PASSENGER_LEAVES_SYSTEM -> {
                    myAgent.totalReturned++
                    val passenger = msg.passenger
                    if (passenger != null) {
                        passenger.leaveTime = mySim().currentTime()
                        if (passenger.arrivalPlace == Place.SERVICE_DESK) {
                            myAgent.sysTimeLeavingPassenger.addSample(passenger.systemTime(), passenger.size)
                        } else {
                            myAgent.sysTimeArrivingPassenger.addSample(passenger.systemTime(), passenger.size)
                        }
                    } else {
                        throw IllegalStateException()
                    }
                    if (!myAgent.canProduceNextArrival() && myAgent.totalGenerated == myAgent.totalReturned) {
                        mySim().stopReplication()
                    }
                }
                else -> throw MessageNotImplementedException(msg, this)
            }
        }
    }
}
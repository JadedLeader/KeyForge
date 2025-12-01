import { useEffect, useState } from "react";
import * as signalR from "@microsoft/signalr";

interface TeamInvite {
    teamInviteId: string; 
    inviteSentBy: string; 
    inviteRecipient: string; 
    inviteStatus: string; 
    inviteCreatedAt: string;
}

export function UseTeamInvites(userEmail: string) { 

    const [teamInvites, setTeamInvites] = useState<TeamInvite[]>([]);

    useEffect(() => { 

        if (!userEmail) return;

        const connection = new signalR.HubConnectionBuilder()
            .withUrl("https://localhost:7088/inviteHub", { transport: signalR.HttpTransportType.WebSockets })
            .withAutomaticReconnect()
            .build();


        connection.start().then(() => {
            console.log("Team Invite SignalR Connected");
            connection.invoke("Register", userEmail);
        });

        connection.on("ReceiveInvite", (invite: TeamInvite) => {
            setTeamInvites((prev) => [...prev, invite]);
        }); 

        return () => {
            connection.stop();
        };

        
    }, [userEmail])

    return teamInvites;

}
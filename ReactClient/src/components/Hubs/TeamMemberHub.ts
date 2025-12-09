import { useEffect, useState } from "react";
import * as signalR from "@microsoft/signalr";

interface TeamMemberAdded { 
    username: string; 
    email: string;
}

export function UseTeamMembers(teamVaultId: string) {

    const [teamMembers, setTeamMembers] = useState<TeamMemberAdded[]>([]);

    useEffect(() => {

        if (!teamVaultId) return;

        const connection = new signalR.HubConnectionBuilder()
            .withUrl("https://localhost:7293/TeamMemberHub", { transport: signalR.HttpTransportType.WebSockets })
            .withAutomaticReconnect()
            .build();


        connection.start().then(() => {
            console.log("Team Members SignalR Connected");
            connection.invoke("JoinVaultGroup", teamVaultId);
        });

        connection.on("ReceiveInvite", (invite: TeamMemberAdded) => {
            setTeamMembers((prev) => [...prev, invite]);
        });

        return () => {
            connection.stop();
        };


    }, [teamVaultId])

    return teamMembers;

}
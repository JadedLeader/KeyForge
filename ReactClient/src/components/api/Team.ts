
interface CreateTeamRequest { 
    teamName: string; 
    teamAcceptingInvites: string; 
    memberCap: number;

}

interface CreateTeamResponse { 
    teamId: string; 
    teamName: string; 
    teamAcceptingInvites: string; 
    createdBy: string; 
    createdAt: string; 
    memberCap: number; 
    success: boolean;
}

type Teams = { 

    teamName: string; 
    teamAcceptingInvitations: string; 
    createdBy: string; 
    createdAt: string; 
    memberCap: Number;
}

function BuildCreateTeamRequest(teamName: string, teamAcceptingInvites: string, memberCap : number) : CreateTeamRequest { 

    const teamRequest: CreateTeamRequest = {
        teamName: teamName,
        teamAcceptingInvites: teamAcceptingInvites,
        memberCap: memberCap
    }; 

    return teamRequest;

}

function BuildCreateTeamResponse(teamId: string, teamName: string, teamAcceptingInvites: string, createdBy: string, createdAt: string, memberCap: number, success: boolean) : CreateTeamResponse { 

    const teamResponse: CreateTeamResponse = {
        teamId: teamId,
        teamName: teamName,
        teamAcceptingInvites: teamAcceptingInvites,
        createdBy: createdBy,
        createdAt: createdAt,
        memberCap: memberCap,
        success: success
    }; 

    return teamResponse; 

}

export async function CreateTeam(teamName: string, teamAcceptingInvites: string, memberCap: number): Promise<CreateTeamResponse> { 


    const buildRequest = BuildCreateTeamRequest(teamName, teamAcceptingInvites, memberCap);

    const createTeamCall = await fetch("/Team/CreateTeam", {
        method: "POST",
        headers: {
            "content-type": "application/json"
        },
        credentials: "include",
        body: JSON.stringify(buildRequest)
    });

    if (!createTeamCall.ok) { 

        const errorText = await createTeamCall.text();

        throw new Error(errorText);

    }

    const jsonBody = await createTeamCall.json();

    console.log(jsonBody);

    return BuildCreateTeamResponse(jsonBody.teamId, jsonBody.teamName, jsonBody.teamAcceptingInvites, jsonBody.createdBy, jsonBody.createdAt, jsonBody.memberCap, jsonBody.success);

}

export async function GetTeams() : Promise<Teams[]> { 


    const getTeams = await fetch("/Team/GetTeams", {
        method: "GET",
        headers: {
            "content-type": "application/json"
        },
        credentials: "include"
    });

    if (!getTeams.ok) { 

        const errorText = await getTeams.text();

        throw new Error(errorText);

    }

    const data: Teams[] = await getTeams.json();

    return data;

}
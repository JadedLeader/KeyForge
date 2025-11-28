
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

interface DeleteTeamRequest { 

    teamId: string;

}

interface DeleteTeamResponse { 

    teamId: string; 
    teamName: string; 
    success: boolean;

}

interface UpdateTeamRequest { 
    teamId: string; 
    newTeamName: string; 
    teamAcceptingInvites: string; 
    memberCap: number;
}

interface UpdateTeamResponse { 
    teamName: string; 
    teamAcceptingInvites: string; 
    memberCap: number; 
    success: boolean;
}

type Teams = { 

    id: string;
    teamName: string; 
    teamAcceptingInvitations: string; 
    createdBy: string; 
    createdAt: string; 
    memberCap: Number;
}

interface GetTeamsResponse { 
    teams: Teams[]; 
    success: boolean;
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

    const data: GetTeamsResponse = await getTeams.json();

    return data.teams || [];

}

function BuildDeleteTeamRequest(teamId: string) : DeleteTeamRequest { 

    const deleteTeam: DeleteTeamRequest = {
        teamId: teamId
    }; 

    return deleteTeam;

}

function BuildDeleteTeamResponse(teamId: string, teamName: string, success: boolean) : DeleteTeamResponse { 

    const deleteTeamResponse: DeleteTeamResponse = {
        teamId: teamId,
        teamName: teamName,
        success: success
    }; 

    return deleteTeamResponse;

}

export async function DeleteTeam(teamId: string) : Promise<DeleteTeamResponse> { 

    const buildRequest = BuildDeleteTeamRequest(teamId);

    const deletedTeam = await fetch("/Team/DeleteTeam", {
        method: "DELETE",
        headers: {
            "content-type": "application/json"
        },
        credentials: "include",
        body: JSON.stringify(buildRequest)
    });

    if (!deletedTeam.ok) { 

        const errorText = await deletedTeam.text();

        throw new Error(errorText);
    }


    const jsonBody = await deletedTeam.json();

    const response = BuildDeleteTeamResponse(jsonBody.teamId, jsonBody.teamName, jsonBody.success);

    return response;

}

function BuildUpdateTeamRequest(teamId: string, newTeamName: string, teamAcceptingInvites: string, memberCap: number, ): UpdateTeamRequest { 

    const teamRequest: UpdateTeamRequest = {
        teamId: teamId,
        newTeamName: newTeamName,
        teamAcceptingInvites: teamAcceptingInvites,
        memberCap: memberCap
    }; 

    return teamRequest;

}

function BuildUpdateTeamResponse(teamName: string, teamAcceptingInvites: string, memberCap: number, success: boolean): UpdateTeamResponse { 

    const updateTeamResponse: UpdateTeamResponse = {
        teamName: teamName,
        teamAcceptingInvites: teamAcceptingInvites,
        memberCap: memberCap,
        success: success
    }; 

    return updateTeamResponse;

}

export async function UpdateTeam(teamId: string, newTeamName: string, teamAcceptingInvites: string, memberCap: number) { 

    const buildRequest = BuildUpdateTeamRequest(teamId, newTeamName, teamAcceptingInvites, memberCap); 

    const updatedTeam = await fetch("/Team/UpdateVault", {
        method: "PUT",
        headers: {
            "content-type": "application/json"
        },
        credentials: "include",
        body: JSON.stringify(buildRequest)

    });

    if (!updatedTeam.ok) { 
        const errorText = await updatedTeam.text();

        throw new Error(errorText);
    }

    const jsonBody = await updatedTeam.json();

    const response = BuildUpdateTeamResponse(jsonBody.teamName, jsonBody.teamAcceptingInvites, jsonBody.memberCap, jsonBody.success);

    return response;

}
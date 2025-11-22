

interface GetUserAccountDetailsResponse {
    username: string;
    email: string;
    success: boolean;
} 

interface SignUpUser {

    username: string;
    password: string;
    email: string;
}

function BuildUsersAccountDetails(username: string, email: string, success: boolean): GetUserAccountDetailsResponse {

    const buildNew: GetUserAccountDetailsResponse = {

        username: username,
        email: email,
        success: success

    };

    return buildNew;

}

export async function GetUserAccountDetails(): Promise<GetUserAccountDetailsResponse> {

    const getAccountDetails = await fetch("/Account/GetUserAccountDetails", {
        method: "GET",
        headers: {
            "content-type": "application/json"
        },
        credentials: "include",
    });

    if (!getAccountDetails.ok) {

        const errorText = await getAccountDetails.text();

        throw new Error(errorText);
    }

    const jsonBody = await getAccountDetails.json();

    console.log(jsonBody);

    const buildResponse = BuildUsersAccountDetails(jsonBody.username, jsonBody.email, jsonBody.success);

    return buildResponse;


}

export async function ReachCreateAccountEndpoint(signingUpUser: SignUpUser): Promise<any> {

    const res = await fetch("/Account/CreateAccounts", {
        method: "POST",
        headers: {
            "Content-Type": "application/json",
        },
        body: JSON.stringify(signingUpUser)
    });

    if (!res.ok) {

        const errorText = await res.text();

        throw new Error(errorText);
    }

    const data = await res.json;

    return data;

}
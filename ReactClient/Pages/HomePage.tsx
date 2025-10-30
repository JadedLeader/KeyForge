import { React, useState, useEffect } from "react";
import { Button } from "@/components/ui/button"
import { Card, CardContent } from "@/components/ui/card"
import "./HomePage.css"; 

interface SilentTokenCycleRequest { 

}


interface SilentTokenCycleResponse { 
    refreshedToken: string; 
    successful: boolean;
}


export function HomePage() {

    const [shortLivedToken, setShortLivedToken] = useState("");

    useEffect(() => {

        const fetchToken = async () => {

            const shortLivedToken = await SilentTokenRefresh();

            setShortLivedToken(shortLivedToken.refreshedToken);
        }; 

        fetchToken();

    }, []);



    async function SilentTokenRefresh(): Promise<SilentTokenCycleResponse> { 


        const buildingRequest = BuildSilentCycleRequest();

        const silentTokenResponse = await fetch("/Auth/SilentShortLivedTokenRefresh", {
            method: "POST",
            headers: {

                "Content-type": "application/json"
            },
            credentials: "include",
            body: JSON.stringify(buildingRequest)
        }); 

        if (!silentTokenResponse.ok) { 

            const errorText = await silentTokenResponse.text();

            throw new Error(errorText);
        }

        return silentTokenResponse;

    }

    function BuildSilentCycleRequest(): SilentTokenCycleRequest { 


        const newCycleRequest: SilentTokenCycleRequest = {

        }; 

        return newCycleRequest;
    }



  return (
    

      <div> 
          <Button variant="outline">Button</Button>
      </div>

    );





}

export default HomePage;
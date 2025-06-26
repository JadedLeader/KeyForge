import { StrictMode } from 'react';
import { createRoot } from 'react-dom/client';
import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import { SignUpPage } from "../Pages/SignUpPage";
import './index.css'
import { SignInPage } from '../Pages/SignInPage';
import { HomePage } from '../Pages/HomePage'; 

createRoot(document.getElementById('root')!).render(
    <StrictMode>
        <BrowserRouter>
            <Routes>

                <Route path="/" element={<SignInPage />}  />

                <Route path="/signup" element={<SignUpPage />} />
                <Route path="/signin" element={<SignInPage />} />
                <Route path="/homepage" element={<HomePage /> } />

                <Route path="*" element={<Navigate to="/signin" />} />

            </Routes>
        </BrowserRouter>

    </StrictMode>,
)

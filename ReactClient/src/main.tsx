import { StrictMode } from 'react';
import { createRoot } from 'react-dom/client';
import { BrowserRouter } from 'react-router-dom';
import { App } from './App';
import { SignUpPage } from "../Pages/SignUpPage";
import './index.css'
import SignInPage from '../Pages/SignInPage';

createRoot(document.getElementById('root')!).render(
    <StrictMode>
        <BrowserRouter>
            <SignUpPage />
            <SignInPage />
        </BrowserRouter>
        
  </StrictMode>,
)

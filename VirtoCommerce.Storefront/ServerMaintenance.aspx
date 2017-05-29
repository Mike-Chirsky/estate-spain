﻿<% Response.StatusCode = 503 %>
<!DOCTYPE html>
<html>
<head>
    <title>Virto Commerce - Maintenance</title>
    <style>
        html
        {
            height: 100%;
            width: 100%;
        }

        #feature
        {
            width: 960px;
            margin: 95px auto 0 auto;
            overflow: auto;
        }

        #content
        {
            font-family: "Segoe UI";
            font-weight: normal;
            font-size: 26px;
            color: #ffffff;
            float: left;
            width: 460px;
            margin-top: 68px;
            margin-left: 0px;
            vertical-align: middle;
        }

            #content h1
            {
                font-family: "Segoe UI Light";
                color: #ffffff;
                font-weight: normal;
                font-size: 70px;
                line-height: 48pt;
                width: 800px;
                padding-bottom: 15px;
            }

        p a, p a:visited, p a:active, p a:hover
        {
            color: #ffffff;
        }

        #content a.button
        {
            background: #0DBCF2;
            border: 1px solid #FFFFFF;
            color: #FFFFFF;
            display: inline-block;
            font-family: Segoe UI;
            font-size: 24px;
            line-height: 46px;
            margin-top: 10px;
            padding: 0 15px 3px;
            text-decoration: none;
        }

            #content a.button img
            {
                float: right;
                padding: 10px 0 0 15px;
            }

            #content a.button:hover
            {
                background: #1C75BC;
            }
    </style>
</head>
<body bgcolor="#00abec">
    <div id="feature">
        <div id="content">
            <h1>Under Maintenance</h1>
            <p>
                The server is under maintenance, please try again later.
            </p>
        </div>
    </div>
</body>
</html>

﻿namespace RoleBasedAuth;

class Role
{
    public string Name { get; set; }
    public Role(string name) => Name = name;
}
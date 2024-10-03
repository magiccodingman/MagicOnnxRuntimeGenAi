using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnnxLibraryConverter.Logic
{
    public class CodeTransformer
    {
        public string TransformDllImports(string inputCode)
        {
            // Parse the input code into a syntax tree
            var syntaxTree = CSharpSyntaxTree.ParseText(inputCode);
            var root = syntaxTree.GetRoot();

            // Extract the class declaration for NativeMethods
            var nativeMethodsClassNode = root.DescendantNodes().OfType<ClassDeclarationSyntax>()
                                .FirstOrDefault(c => c.Identifier.Text == "NativeMethods");

            if (nativeMethodsClassNode == null)
            {
                throw new InvalidOperationException("Could not find NativeMethods class in the provided code.");
            }

            // Extract all method declarations with DllImport attributes
            var dllImportMethods = nativeMethodsClassNode.DescendantNodes()
                                       .OfType<MethodDeclarationSyntax>()
                                       .Where(m => m.AttributeLists
                                                      .Any(attrList => attrList.Attributes
                                                          .Any(attr => attr.Name.ToString().Contains("DllImport"))));

            // Create the MagicNativeMethods class builder with the custom comment
            var magicNativeMethodsClass = SyntaxFactory.ClassDeclaration("MagicNativeMethods")
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword), SyntaxFactory.Token(SyntaxKind.PartialKeyword))
                .AddMembers(CreateMagicNativeMethodsConstructor())
                .NormalizeWhitespace();

            // Transform each method with DllImport into delegate-based code and add to the new class
            foreach (var method in dllImportMethods)
            {
                var methodName = method.Identifier.Text;
                var parameters = method.ParameterList.Parameters;
                var returnType = method.ReturnType;

                // Determine if the method requires an unsafe context by checking for pointer types
                bool requiresUnsafe = returnType.ToString().Contains("*") || parameters.Any(p => p.Type.ToString().Contains("*"));

                // Create delegate name
                var delegateName = $"{methodName}Delegate";

                // Build the delegate declaration
                var delegateDeclaration = SyntaxFactory.DelegateDeclaration(returnType, delegateName)
                    .WithParameterList(method.ParameterList)
                    .WithModifiers(SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.PrivateKeyword)));

                // If the delegate requires 'unsafe', add the modifier
                if (requiresUnsafe)
                {
                    delegateDeclaration = delegateDeclaration.AddModifiers(SyntaxFactory.Token(SyntaxKind.UnsafeKeyword));
                }

                // Build the method body, handling 'out' and 'ref' keywords
                var arguments = SyntaxFactory.SeparatedList(
                    parameters.Select(p =>
                        SyntaxFactory.Argument(SyntaxFactory.IdentifierName(p.Identifier.Text))
                            .WithRefOrOutKeyword(p.Modifiers.Any(m => m.IsKind(SyntaxKind.OutKeyword))
                                ? SyntaxFactory.Token(SyntaxKind.OutKeyword)
                                : p.Modifiers.Any(m => m.IsKind(SyntaxKind.RefKeyword))
                                    ? SyntaxFactory.Token(SyntaxKind.RefKeyword)
                                    : default(SyntaxToken))));

                // Check if the method return type is void
                bool isVoid = returnType.ToString() == "void";

                // Build the method body
                var methodBody = isVoid
                    ? SyntaxFactory.ExpressionStatement(
                        SyntaxFactory.InvocationExpression(
                            SyntaxFactory.IdentifierName("method"))
                        .WithArgumentList(SyntaxFactory.ArgumentList(arguments)))
                    : (StatementSyntax)SyntaxFactory.ReturnStatement(
                        SyntaxFactory.InvocationExpression(
                            SyntaxFactory.IdentifierName("method"))
                        .WithArgumentList(SyntaxFactory.ArgumentList(arguments)));

                // Determine the method modifiers (unsafe if required)
                var methodModifiers = SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.PublicKeyword));
                if (requiresUnsafe)
                {
                    methodModifiers = methodModifiers.Add(SyntaxFactory.Token(SyntaxKind.UnsafeKeyword));
                }

                var methodDeclaration = SyntaxFactory.MethodDeclaration(returnType, methodName)
                    .WithParameterList(method.ParameterList)
                    .WithModifiers(methodModifiers)
                    .WithBody(SyntaxFactory.Block(
                        SyntaxFactory.LocalDeclarationStatement(
                            SyntaxFactory.VariableDeclaration(
                                SyntaxFactory.IdentifierName("var"),
                                SyntaxFactory.SeparatedList(new[]
                                {
                                SyntaxFactory.VariableDeclarator(
                                    SyntaxFactory.Identifier("method"))
                                    .WithInitializer(SyntaxFactory.EqualsValueClause(
                                        SyntaxFactory.InvocationExpression(
                                            SyntaxFactory.GenericName(SyntaxFactory.Identifier("GetNativeMethod"))
                                                .WithTypeArgumentList(SyntaxFactory.TypeArgumentList(
                                                    SyntaxFactory.SingletonSeparatedList<TypeSyntax>(
                                                        SyntaxFactory.IdentifierName(delegateName)))))
                                            .WithArgumentList(SyntaxFactory.ArgumentList(
                                                SyntaxFactory.SingletonSeparatedList(
                                                    SyntaxFactory.Argument(SyntaxFactory.LiteralExpression(
                                                        SyntaxKind.StringLiteralExpression,
                                                        SyntaxFactory.Literal(methodName))))))))
                                }))),
                        methodBody // Add the correct method body based on the return type
                    ));

                // Add the delegate and method to the MagicNativeMethods class
                magicNativeMethodsClass = magicNativeMethodsClass.AddMembers(delegateDeclaration, methodDeclaration);
            }

            // Convert the modified class to code and return only the MagicNativeMethods class
            return magicNativeMethodsClass.NormalizeWhitespace().ToFullString();
        }

        // Create the constructor for MagicNativeMethods class
        private ConstructorDeclarationSyntax CreateMagicNativeMethodsConstructor()
        {
            var constructor = SyntaxFactory.ConstructorDeclaration("MagicNativeMethods")
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword))
                .WithParameterList(
                    SyntaxFactory.ParameterList(
                        SyntaxFactory.SingletonSeparatedList(
                            SyntaxFactory.Parameter(SyntaxFactory.Identifier("hardwareType"))
                            .WithType(SyntaxFactory.IdentifierName("HardwareType")))))
                .WithBody(SyntaxFactory.Block(
                    SyntaxFactory.ExpressionStatement(
                        SyntaxFactory.AssignmentExpression(
                            SyntaxKind.SimpleAssignmentExpression,
                            SyntaxFactory.IdentifierName("_hardwareType"),
                            SyntaxFactory.IdentifierName("hardwareType"))),
                    SyntaxFactory.ExpressionStatement(
                        SyntaxFactory.InvocationExpression(SyntaxFactory.IdentifierName("SetLibraryPath")))));

            return constructor;
        }
    }
}

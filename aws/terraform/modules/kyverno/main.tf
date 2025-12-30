resource "helm_release" "kyverno" {
  name             = var.helm_release_name
  repository       = var.helm_repository
  chart            = var.helm_chart
  version          = var.helm_version
  namespace        = var.namespace
  create_namespace = var.create_namespace
}

resource "kubectl_manifest" "kyverno" {
  yaml_body = <<-YAML
apiVersion: kyverno.io/v1
kind: ClusterPolicy
metadata:
  name: ${var.policy_name}
  annotations:
    pod-policies.kyverno.io/autogen-controllers: none
spec:
  background: false
  rules:
  - name: replace-image-registry
    match:
      any:
      - resources:
          kinds:
          - Pod
    mutate:
      foreach:
      - list: "request.object.spec.containers"
        patchStrategicMerge:
          spec:
            containers:
            - name: "{{ element.name }}"
              image: "{{ regex_replace_all('^${var.image_prefix}', element.image, '${var.ecr_registry}/') }}"
  YAML

  depends_on = [helm_release.kyverno]
}
